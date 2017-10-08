﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;
using Prime.Core.Exchange;
using Prime.Plugins.Services.Base;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.BitMex
{
    public class BitMexProvider : IExchangeProvider, IWalletService, IOhlcProvider, IApiProvider, IOrderBookProvider
    {
        private static readonly ObjectId IdHash = "prime:bitmex".GetObjectIdHashCode();

        private const String BitMexApiUrl = "https://www.bitmex.com/api/v1";

        private static readonly string _pairs = "btcusd";
        private const decimal ConversionRate = 0.00000001m;

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);

        public BitMexProvider()
        {
            Network = new Network("BitMex");
            Disabled = false;
            AggregatorName = null;
            Title = Network.Name;
            Id = IdHash;
            Priority = 100;
            GetApiConfiguration = ApiConfiguration.Standard2;

            CanGenerateDepositAddress = false;
            CanMultiDepositAddress = false;
        }

        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(150, 5, 300, 5);
        public IRateLimiter RateLimiter => Limiter;

        private string ConvertToBitMexInterval(TimeResolution market)
        {
            switch (market)
            {
                case TimeResolution.Minute:
                    return "1m";
                case TimeResolution.Hour:
                    return "1h";
                case TimeResolution.Day:
                    return "1d";
                default:
                    throw new ArgumentOutOfRangeException(nameof(market), market, null);
            }
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = GetApi<IBitMexApi>(context);

            var resolution = ConvertToBitMexInterval(context.Market);
            var startDate = context.Range.UtcFrom;
            var endDate = context.Range.UtcTo;

            var r = await api.GetTradeHistory(context.Pair.Asset1.ToRemoteCode(this), resolution, startDate, endDate);

            var ohlc = new OhlcData(context.Market);
            var seriesId = OhlcResolutionAdapter.GetHash(context.Pair, context.Market, Network);

            foreach (var instrActive in r)
            {
                ohlc.Add(new OhlcEntry(seriesId, instrActive.timestamp, this)
                {
                    Open = (double)instrActive.open,
                    Close = (double)instrActive.close,
                    Low = (double)instrActive.low,
                    High = (double)instrActive.high,
                    VolumeTo = (long)instrActive.volume,
                    VolumeFrom = (long)instrActive.volume,
                    WeightedAverage = (double) (instrActive.vwap ?? 0) // BUG: what to set if vwap is NULL?
                });
            }

            return ohlc;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return new BitMexCodeConverter();
        }

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = GetApi<IBitMexApi>(context);
            var r = (await api.GetLatestPriceAsync(context.Pair.Asset1.ToRemoteCode(this))).FirstOrDefault();

            if (r == null)
                throw new ApiResponseException("No price data found", this);

            if (r.timestamp.Kind != DateTimeKind.Utc)
                throw new ApiResponseException("Time is not in UTC format", this);

            // TODO: Check this. How to handle NULL in last price value?
            if (r.lastPrice.HasValue == false)
                throw new ApiResponseException("No last price for currency", this);

            var latestPrice = new LatestPrice
            {
                BaseAsset = context.Pair.Asset1,
                Price = new Money(r.lastPrice.Value, context.Pair.Asset2),
                UtcCreated = r.timestamp
            };

            return latestPrice;
        }

        public async Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context)
        {
            if(context.Assets.Count < 1) 
                throw new ArgumentException("The number of target assets should be greater than 0");

            var api = GetApi<IBitMexApi>(context);
            var r = await api.GetLatestPricesAsync();

            if(r == null || r.Count < 1)
                throw new ApiResponseException("No prices data found", this);

            var pricesList = new List<Money>();

            foreach (var asset in context.Assets)
            {
                var remote = context.BaseAsset.ToRemoteCode(this);
                var pairCode = (remote + asset.ToRemoteCode(this)).ToLower();

                var data = r.FirstOrDefault(x =>
                    x.symbol.ToLower().Equals(pairCode)
                );

                if (data == null || data.lastPrice.HasValue == false)
                    throw new ApiResponseException("No price returned for selected currency");

                pricesList.Add(new Money(data.lastPrice.Value, data.quoteCurrency.ToAsset(this)));
            }

            var latestPrices = new LatestPrices()
            {
                BaseAsset = context.BaseAsset,
                Prices = pricesList
            };

            return latestPrices;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            throw new NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new NotImplementedException();
        }

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var t = new Task<AssetPairs>(() => Pairs);
            t.RunSynchronously();

            return t;

            // This code fetches all pairs including futures which are not supported for this moment.

            /* var api = GetApi<IBitMexApi>(context);
            var r = await api.GetInstrumentsActive();
            var aps = new AssetPairs();
            foreach (var i in r)
            {
                var ap = new AssetPair(i.underlying.ToAsset(this), i.quoteCurrency.ToAsset(this));
                aps.Add(ap);
            } */
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = GetApi<IBitMexApi>(context);
            var r = await api.GetUserInfoAsync();
            return r != null;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = GetApi<IBitMexApi>(context);

            var remoteAssetCode = context.Asset.ToRemoteCode(this);
            var depositAddress = await api.GetUserDepositAddressAsync(remoteAssetCode);

            depositAddress = depositAddress.Trim('\"');

            var addresses = new WalletAddresses();
            var walletAddress = new WalletAddress(this, context.Asset) {Address = depositAddress};

            addresses.Add(walletAddress);

            return addresses;
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = GetApi<IBitMexApi>(context);
            var addresses = new WalletAddresses();

            foreach (var assetPair in Pairs)
            {
                var adjustedCode = AdjustAssetCode(assetPair.Asset1.ShortCode);

                var depositAddress = await api.GetUserDepositAddressAsync(adjustedCode);

                depositAddress = depositAddress.Trim('\"');

                // BUG: how to convert XBt from Pairs to BTC?
                addresses.Add(new WalletAddress(this, Asset.Btc)
                {
                    Address = depositAddress
                });
            }

            return addresses;
        }

        private string AdjustAssetCode(string input)
        {
            // TODO: should be re-factored.
            var config = new Dictionary<string, string>();

            config.Add("XBT", "XBt");

            return config.ContainsKey(input) ? config[input] : null;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = GetApi<IBitMexApi>(context);

            var r = await api.GetUserWalletInfoAsync("XBt");

            var results = new BalanceResults(this);

            var btcAmount = (decimal)ConversionRate * r.amount;

            var c = Asset.Btc;

            var balance = new BalanceResult(c);
            balance.Balance = new Money(btcAmount, c);
            balance.Available = new Money(btcAmount, c);
            balance.Reserved = new Money(0, c);

            results.Add(balance);

            return results;
        }

        public async Task<OrderBook> GetOrderBookLive(OrderBookLiveContext context)
        {
            var api = GetApi<IBitMexApi>(context);

            var pairCode = GetBitMexTicker(context.Pair);

            var r = await api.GetOrderBookAsync(pairCode, 1);

            var buys = r.Where(x => x.side.ToLower().Equals("buy")).OrderBy(x => x.id).ToArray();
            var sells = r.Where(x => x.side.ToLower().Equals("sell")).OrderBy(x => x.id).ToArray();

            var buyEntry = buys.FirstOrDefault();
            var sellEntry = sells.FirstOrDefault();

            if (buys.Length != 1 || buys.Length != sells.Length)
                throw new ApiResponseException("Incorrect number of order book records returned", this);

            if (buyEntry == null || sellEntry == null)
                throw new ApiResponseException("Order book data is empty", this);

            var orderBook = new OrderBook();
            orderBook.Add(new OrderBookRecord()
            {
                Data = new BidAskData()
                {
                    Price = new Money(sellEntry.price, context.Pair.Asset2),
                    Time = DateTime.Now,
                    Volume = sellEntry.size
                },
                Type = OrderBookType.Ask
            });
            orderBook.Add(new OrderBookRecord()
            {
                Data = new BidAskData()
                {
                    Price = new Money(buyEntry.price, context.Pair.Asset2),
                    Time = DateTime.Now,
                    Volume = buyEntry.size
                },
                Type = OrderBookType.Bid
            });

            return orderBook;
        }

        public async Task<OrderBook> GetOrderBookHistory(OrderBookContext context)
        {
            var api = GetApi<IBitMexApi>(context);

            var pairCode = GetBitMexTicker(context.Pair);

            var r = await api.GetOrderBookAsync(pairCode, context.Depth);

            var buys = r.Where(x => x.side.ToLower().Equals("buy")).OrderBy(x => x.id).ToArray();
            var sells = r.Where(x => x.side.ToLower().Equals("sell")).OrderBy(x => x.id).ToArray();

            var orderBook = new OrderBook();

            foreach (var buy in buys)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Bid,
                    Data = new BidAskData()
                    {
                        Price = new Money(buy.price, context.Pair.Asset2),
                        Time = DateTime.Now, // Since it returnes current state of OrderBook, date time is set to Now.
                        Volume = buy.size
                    }
                });
            }

            foreach (var sell in sells)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Ask,
                    Data = new BidAskData()
                    {
                        Price = new Money(sell.price, context.Pair.Asset2),
                        Time = DateTime.Now,
                        Volume = sell.size
                    }
                });
            }

            return orderBook;
        }

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return RestClient.For<IBitMexApi>(BitMexApiUrl) as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);

            return RestClient.For<IBitMexApi>(BitMexApiUrl, new BitMexAuthenticator(key).GetRequestModifier) as T;
        }

        private string GetBitMexTicker(AssetPair pair)
        {
            return $"{pair.Asset1.ToRemoteCode(this)}{pair.Asset2.ToRemoteCode(this)}".ToUpper();
        }

        public ApiConfiguration GetApiConfiguration { get; }

        public bool CanMultiDepositAddress { get; }

        public bool CanGenerateDepositAddress { get; }

        public ObjectId Id { get; }

        public Network Network { get; }

        public bool Disabled { get; }

        public int Priority { get; }

        public string AggregatorName { get; }

        public string Title { get; }

    }
}
