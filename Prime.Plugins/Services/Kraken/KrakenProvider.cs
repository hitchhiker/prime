﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Core;
using Prime.Core.Exchange;
using Prime.Plugins.Services.Base;
using Prime.Plugins.Services.Kraken.Converters;
using Prime.Utility;
using RestEase;
using AssetPair = Prime.Core.AssetPair;

namespace Prime.Plugins.Services.Kraken
{
    public class KrakenProvider : IExchangeProvider, IWalletService, IOhlcProvider, IApiProvider, IOrderBookProvider
    {
        private const String KrakenApiUrl = "https://api.kraken.com/0";

        public Network Network { get; } = new Network("Kraken");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        private JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                Converters = { new OhlcJsonConverter() }
            };
        }

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return new RestClient(KrakenApiUrl)
            {
                JsonSerializerSettings = CreateJsonSerializerSettings()
            }.For<IKrakenApi>() as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);

            return new RestClient(KrakenApiUrl, new KrakenAuthenticator(key).GetRequestModifier)
            {
                JsonSerializerSettings = CreateJsonSerializerSettings()
            }.For<IKrakenApi>() as T;
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = GetApi<IKrakenApi>(context);
            var body = CreateKrakenBody();

            var r = await api.GetBalancesAsync(body);

            CheckResponseErrors(r);

            return r != null;
        }

        private static readonly ObjectId IdHash = "prime:kraken".GetObjectIdHashCode();

        public ObjectId Id => IdHash;

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = GetApi<IKrakenApi>(context);

            var remoteCode = GetKrakenTicker(context.Pair);

            var r = await api.GetTickerInformationAsync(remoteCode);

            CheckResponseErrors(r);

            // TODO: Check, price is taken from "last trade closed array(<price>, <lot volume>)".
            var money = new Money(r.result.FirstOrDefault().Value.c[0], context.Pair.Asset2);
            var price = new LatestPrice()
            {
                Price = money,
                BaseAsset = context.Pair.Asset1,
                UtcCreated = DateTime.Now
            };

            return price;
        }

        public async Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context)
        {
            var api = GetApi<IKrakenApi>(context);
            var prices = new List<Money>();

            foreach (var asset in context.Assets)
            {
                var pair = new AssetPair(context.BaseAsset, asset);
                var remoteCode = GetKrakenTicker(pair);

                var r = await api.GetTickerInformationAsync(remoteCode);

                CheckResponseErrors(r);

                prices.Add(new Money(r.result.FirstOrDefault().Value.c[0], asset));
            }

            var latestPrices = new LatestPrices()
            {
                BaseAsset = context.BaseAsset,
                Prices = prices
            };

            return latestPrices;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = GetApi<IKrakenApi>(context);

            var r = await api.GetAssetPairsAsync();

            CheckResponseErrors(r);

            var assetPairs = new AssetPairs();

            foreach (var assetPair in r.result)
            {
                var ticker = assetPair.Key;
                var first = assetPair.Value.base_c;
                var second = ticker.Replace(first, "");

                assetPairs.Add(new AssetPair(first, second, this));
            }

            return assetPairs;
        }

        public bool CanMultiDepositAddress { get; } = false;
        public bool CanGenerateDepositAddress { get; } = true;

        private Dictionary<string, object> CreateKrakenBody()
        {
            var body = new Dictionary<string, object>();
            var nonce = BaseAuthenticator.GetLongNonce();

            body.Add("nonce", nonce);

            return body;
        }

        private void CheckResponseErrors(KrakenSchema.ErrorResponse response)
        {
            if (response.error.Length > 0)
            {
                throw new ApiResponseException(response.error[0], this);
            }
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = GetApi<IKrakenApi>(context);

            var body = CreateKrakenBody();

            var r = await api.GetBalancesAsync(body);

            CheckResponseErrors(r);

            var results = new BalanceResults(this);

            foreach (var pair in r.result)
            {
                var asset = pair.Key.ToAsset(this);
                var money = new Money(pair.Value, asset);

                results.Add(new BalanceResult(asset)
                {
                    Available = money,
                    Balance = money,
                    Reserved = 0
                });
            }

            return results;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return KrakenCodeConverterBase.I;
        }

        public async Task<string> GetFundingMethod(NetworkProviderPrivateContext context, Asset asset)
        {
            var api = GetApi<IKrakenApi>(context);

            var body = CreateKrakenBody();
            body.Add("asset", asset.ToRemoteCode(this));

            try
            {
                var r = await api.GetDepositMethodsAsync(body);

                CheckResponseErrors(r);

                return r?.result?.FirstOrDefault()?.method;
            }
            catch (ApiResponseException e)
            {
                if (e.Message.ToLower().Contains("internal error"))
                {
                    throw new ApiResponseException(
                        "Kraken internal error. Possible reason is unverified account (Tier 1 is required).");
                }
            }

            return null;
        }

        private async Task<WalletAddresses> GetAddressesLocal(IKrakenApi api, string fundingMethod, Asset asset)
        {
            var body = CreateKrakenBody();

            // BUG: do we need "aclass"?
            //body.Add("aclass", asset.ToRemoteCode(this));
            body.Add("asset", asset.ToRemoteCode(this));
            body.Add("method", fundingMethod);
            body.Add("new", false);

            var r = await api.GetDepositAddresses(body);
            CheckResponseErrors(r);

            var walletAddresses = new WalletAddresses();

            foreach (var addr in r.result)
            {
                var walletAddress = new WalletAddress(this, asset)
                {
                    Address = addr.address
                };

                if (addr.expiretm != 0)
                {
                    var time = addr.expiretm.ToUtcDateTime();
                    walletAddress.ExpiresUtc = time;
                }

                walletAddresses.Add(new WalletAddress(this, asset) { Address = addr.address });
            }

            return walletAddresses;
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = GetApi<IKrakenApi>(context);
            var assets = await GetAssetPairs(context);

            var addresses = new WalletAddresses();

            foreach (var pair in assets)
            {
                var fundingMethod = await GetFundingMethod(context, pair.Asset1);

                if (fundingMethod == null)
                    throw new NullReferenceException("No funding method is found");

                var localAddresses = await GetAddressesLocal(api, fundingMethod, pair.Asset1);

                addresses.AddRange(localAddresses);
            }

            return addresses;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = GetApi<IKrakenApi>(context);

            var fundingMethod = await GetFundingMethod(context, context.Asset);

            if (fundingMethod == null)
                throw new NullReferenceException("No funding method is found");

            var addresses = await GetAddressesLocal(api, fundingMethod, context.Asset);

            return addresses;
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = GetApi<IKrakenApi>(context);

            var krakenTimeInterval = ConvertToKrakenInterval(context.Market);

            // BUG: "since" is not implemented. Need to be checked.
            var r = await api.GetOhlcDataAsync(GetKrakenTicker(context.Pair), krakenTimeInterval);

            CheckResponseErrors(r);

            var ohlc = new OhlcData(context.Market);
            var seriesId = OhlcResolutionAdapter.GetHash(context.Pair, context.Market, Network);

            if (r.result.pairs.Count != 0)
            {
                foreach (var ohlcResponse in r.result.pairs.FirstOrDefault().Value.OrderByDescending(x => x.time))
                {
                    var time = ((long)ohlcResponse.time).ToUtcDateTime();

                    // BUG: ohlcResponse.volume is double ~0.2..10.2, why do we cast to long?
                    ohlc.Add(new OhlcEntry(seriesId, time, this)
                    {
                        Open = (double)ohlcResponse.open,
                        Close = (double)ohlcResponse.close,
                        Low = (double)ohlcResponse.low,
                        High = (double)ohlcResponse.high,
                        VolumeTo = (long)ohlcResponse.volume, // Cast to long should be revised.
                        VolumeFrom = (long)ohlcResponse.volume,
                        WeightedAverage = (double)ohlcResponse.vwap // Should be checked.
                    });
                }
            }
            else
            {
                throw new ApiResponseException("No OHLC data received", this);
            }

            return ohlc;
        }

        private KrakenTimeInterval ConvertToKrakenInterval(TimeResolution resolution)
        {
            // BUG: Kraken does not support None, MS, S. At this moment it will throw ArgumentOutOfRangeException.
            switch (resolution)
            {
                case TimeResolution.Minute:
                    return KrakenTimeInterval.Minute1;
                case TimeResolution.Hour:
                    return KrakenTimeInterval.Hours1;
                case TimeResolution.Day:
                    return KrakenTimeInterval.Day1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        private (decimal Price, decimal Volume, DateTime TimeStamp) GetBidAskData(string[] dataArray)
        {
            if(dataArray == null)
                throw new ApiResponseException("Invalid order book data response", this);

            decimal price;
            decimal volume;
            long timeStampUnix;
            DateTime timeStamp;

            if (!decimal.TryParse(dataArray[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out price))
                throw new ApiResponseException("Incorrect order book data format", this);

            if (!decimal.TryParse(dataArray[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out volume))
                throw new ApiResponseException("Incorrect order book data format", this);

            if(!long.TryParse(dataArray[2], out timeStampUnix))
                throw new ApiResponseException("Incorrect order book data format", this);

            timeStamp = timeStampUnix.ToUtcDateTime();

            return (price, volume, timeStamp);
        }

        public async Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            var api = GetApi<IKrakenApi>(context);
            var orderBook = await GetOrderBookLocal(api, context.Pair, context.MaxRecordsCount);

            return orderBook;
        }

        private async Task<OrderBook> GetOrderBookLocal(IKrakenApi api, AssetPair assetPair, int? maxCount)
        {
            var pair = assetPair;
            var remotePair = new AssetPair(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this));
            
            var r = await api.GetOrderBook(remotePair.TickerSimple(), maxCount ?? 0);

            CheckResponseErrors(r);

            var data = r.result.FirstOrDefault();
            var orderBook = new OrderBook();

            var asks = maxCount.HasValue ? data.Value.asks.Take(maxCount.Value / 2).ToArray() : data.Value.asks;
            var bids = maxCount.HasValue ? data.Value.bids.Take(maxCount.Value / 2).ToArray(): data.Value.bids;

            foreach (var askArray in asks)
            {
                var askData = GetBidAskData(askArray);

                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData(new Money(askData.Price, assetPair.Asset2), askData.Price, askData.TimeStamp),
                    Type = OrderBookType.Ask
                });
            }

            foreach (var bidArray in bids)
            {
                var bidData = GetBidAskData(bidArray);

                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData(new Money(bidData.Price, assetPair.Asset2), bidData.Price, bidData.TimeStamp),
                    Type = OrderBookType.Bid
                });
            }

            return orderBook;
        }

        public string GetKrakenTicker(AssetPair pair)
        {
            return $"{pair.Asset1.ToRemoteCode(this)}{pair.Asset2.ToRemoteCode(this)}";
        }
    }
}