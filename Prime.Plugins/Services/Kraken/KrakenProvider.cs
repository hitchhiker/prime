using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Plugins.Services.Base;
using Prime.Plugins.Services.Kraken.Converters;
using Prime.Utility;
using RestEase;
using AssetPair = Prime.Common.AssetPair;

namespace Prime.Plugins.Services.Kraken
{

    public class KrakenProvider : IBalanceProvider, IOhlcProvider, IOrderBookProvider, IPublicPriceProvider, IPublicPricesProvider, IPublicPriceStatistics, IAssetPairsProvider, IDepositProvider
    {
        private const String KrakenApiUrl = "https://api.kraken.com/0";

        private RestApiClientProvider<IKrakenApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Kraken");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;

        // 'Ledger/trade history calls increase the counter by 2. ... Tier 2 users have a maximum of 15 and their count gets reduced by 1 every 3 seconds.'
        // Worst case scenario is considered here.
        // https://www.kraken.com/help/api
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(10, 1);
        public IRateLimiter RateLimiter => Limiter;


        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => true;

        private static readonly ObjectId IdHash = "prime:kraken".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public KrakenProvider()
        {
            ApiProvider = new RestApiClientProvider<IKrakenApi>(KrakenApiUrl, this, k => new KrakenAuthenticator(k).GetRequestModifier)
            {
                JsonSerializerSettings = CreateJsonSerializerSettings()
            };
        }

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        private JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                Converters = { new OhlcJsonConverter() }
            };
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreateKrakenBody();

            var r = await api.GetBalancesAsync(body).ConfigureAwait(false);

            CheckResponseErrors(r);

            return r != null;
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var remoteCode = GetKrakenTicker(context.Pair);

            var r = await api.GetTickerInformationAsync(remoteCode).ConfigureAwait(false);

            CheckResponseErrors(r);

            var ticker = r.result.FirstOrDefault().Value;

            // TODO: test statistics.
            return new MarketPrice(Network,  context.Pair, ticker.c[0])
            {
                PriceStatistics = new PriceStatistics(context.QuoteAsset, ticker.v[1], null, ticker.a[0], ticker.b[0], ticker.l[1], ticker.h[1])
            };
        }

        public Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairsCsv = context.Pairs.Aggregate("", (s, pair) => s += GetKrakenTicker(pair) + ",").TrimEnd(',');

            var r = await api.GetTickerInformationAsync(pairsCsv).ConfigureAwait(false);

            CheckResponseErrors(r);

            var prices = new MarketPricesResult();
            foreach (var pair in context.Pairs)
            {
                var rTicker = r.result.Where(x => ComparePairs(pair, x.Key)).ToArray();

                if (!rTicker.Any())
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.MarketPrices.Add(new MarketPrice(Network, pair, rTicker.First().Value.c[0]));
            }

            return prices;
        }

        private bool ComparePairs(AssetPair pair, string krakenPairCode)
        {
            var pattern = @"^(([X](?<asset10>\w{3}))|((?<asset11>.\w{3})))[XZ](?<asset20>\w{3})$";
            var matches = Regex.Match(krakenPairCode, pattern);

            if (!matches.Success || !matches.Groups["asset20"].Success || (!matches.Groups["asset10"].Success && !matches.Groups["asset11"].Success))
                return false;

            var krakenPair = new AssetPair(
                matches.Groups["asset10"].Success ? matches.Groups["asset10"].Value : matches.Groups["asset11"].Value,
                matches.Groups["asset20"].Value,
                this
                );

            return pair.Equals(krakenPair);
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            CheckResponseErrors(r);

            var assetPairs = new AssetPairs();

            // BUG: USDTUSD fix. USDTUSD -> USD.T issue.
            var usdtUsd = "USDTUSD";
            if (r.result.Any(x => x.Value.altname.Equals(usdtUsd)))
            {
                r.result.RemoveAll(x => x.Value.altname.Equals(usdtUsd));
                assetPairs.Add(new AssetPair("USDT", "USD"));
            }

            foreach (var assetPair in r.result.Where(x => !x.Key.ToLower().EndsWith(".d")).OrderBy(x => x.Value.altname.Length))
            {
                var pair = AssetsUtilities.GetAssetPair(assetPair.Value.altname, assetPairs);

                if (!pair.HasValue)
                    continue;

                assetPairs.Add(new AssetPair(pair.Value.AssetCode1.ToAsset(this), pair.Value.AssetCode2.ToAsset(this)));
            }

            return assetPairs;
        }

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
            var api = ApiProvider.GetApi(context);

            var body = CreateKrakenBody();

            var r = await api.GetBalancesAsync(body).ConfigureAwait(false);

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

        public async Task<string> GetFundingMethodAsync(NetworkProviderPrivateContext context, Asset asset)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreateKrakenBody();
            body.Add("asset", asset.ToRemoteCode(this));

            try
            {
                var r = await api.GetDepositMethodsAsync(body).ConfigureAwait(false);

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

        private async Task<WalletAddresses> GetAddressesLocalAsync(IKrakenApi api, string fundingMethod, Asset asset, bool generateNew = false)
        {
            var body = CreateKrakenBody();

            // BUG: do we need "aclass"?
            //body.Add("aclass", asset.ToRemoteCode(this));
            body.Add("asset", asset.ToRemoteCode(this));
            body.Add("method", fundingMethod);
            body.Add("new", generateNew);

            var r = await api.GetDepositAddressesAsync(body).ConfigureAwait(false);
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
            var api = ApiProvider.GetApi(context);
            var assets = await GetAssetPairsAsync(context).ConfigureAwait(false);

            var addresses = new WalletAddresses();

            foreach (var pair in assets)
            {
                var fundingMethod = await GetFundingMethodAsync(context, pair.Asset1).ConfigureAwait(false);

                if (fundingMethod == null)
                    throw new NullReferenceException("No funding method is found");

                var localAddresses = await GetAddressesLocalAsync(api, fundingMethod, pair.Asset1).ConfigureAwait(false);

                addresses.AddRange(localAddresses);
            }

            return addresses;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var fundingMethod = await GetFundingMethodAsync(context, context.Asset).ConfigureAwait(false);

            if (fundingMethod == null)
                throw new NullReferenceException("No funding method is found");

            var addresses = await GetAddressesLocalAsync(api, fundingMethod, context.Asset).ConfigureAwait(false);

            return addresses;
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = ApiProvider.GetApi(context);

            var krakenTimeInterval = ConvertToKrakenInterval(context.Market);

            // BUG: "since" is not implemented. Need to be checked.
            var r = await api.GetOhlcDataAsync(GetKrakenTicker(context.Pair), krakenTimeInterval).ConfigureAwait(false);

            CheckResponseErrors(r);

            var ohlc = new OhlcData(context.Market);
            var seriesId = OhlcUtilities.GetHash(context.Pair, context.Market, Network);

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
            if (dataArray == null)
                throw new ApiResponseException("Invalid order book data response", this);

            decimal price;
            decimal volume;
            long timeStampUnix;
            DateTime timeStamp;

            if (!decimal.TryParse(dataArray[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out price))
                throw new ApiResponseException("Incorrect order book data format", this);

            if (!decimal.TryParse(dataArray[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out volume))
                throw new ApiResponseException("Incorrect order book data format", this);

            if (!long.TryParse(dataArray[2], out timeStampUnix))
                throw new ApiResponseException("Incorrect order book data format", this);

            timeStamp = timeStampUnix.ToUtcDateTime();

            return (price, volume, timeStamp);
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var orderBook = await GetOrderBookLocalAsync(api, context.Pair, context.MaxRecordsCount).ConfigureAwait(false);

            return orderBook;
        }

        private async Task<OrderBook> GetOrderBookLocalAsync(IKrakenApi api, AssetPair assetPair, int? maxCount)
        {
            var pair = assetPair;
            var remotePair = new AssetPair(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this));

            var r = await api.GetOrderBookAsync(remotePair.TickerSimple(), maxCount ?? 0).ConfigureAwait(false);

            CheckResponseErrors(r);

            var data = r.result.FirstOrDefault();
            var orderBook = new OrderBook();

            var asks = maxCount.HasValue ? data.Value.asks.Take(maxCount.Value / 2).ToArray() : data.Value.asks;
            var bids = maxCount.HasValue ? data.Value.bids.Take(maxCount.Value / 2).ToArray() : data.Value.bids;

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

        public async Task<VolumeResult> GetVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);

            var remoteCode = GetKrakenTicker(context.Pair);

            var r = await api.GetTickerInformationAsync(remoteCode).ConfigureAwait(false);

            CheckResponseErrors(r);

            return new VolumeResult()
            {
                Pair = context.Pair,
                Volume = r.result.FirstOrDefault().Value.v[0],
                Period = VolumePeriod.Day
            };
        }
    }
}