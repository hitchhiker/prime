using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Plugins.Services.Binance
{
    public class BinanceProvider : IOrderBookProvider, IBalanceProvider, IOhlcProvider, IPublicPricesProvider, IPublicPriceProvider, IAssetPairsProvider, IAssetPairVolumeProvider
    {
        // public const string BinanceApiVersion = "v1";
        public const string BinanceApiUrl = "https://www.binance.com/api";

        private static readonly ObjectId IdHash = "prime:bitflyer".GetObjectIdHashCode();

        private RestApiClientProvider<IBinanceApi> ApiProvider { get; }

        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("Binance");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;
        
        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BinanceProvider()
        {
            ApiProvider = new RestApiClientProvider<IBinanceApi>(BinanceApiUrl, this, k => new BinanceAuthenticator(k).GetRequestModifier);
        }

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerSimple();

            var interval = ConvertToBinanceInterval(context.Market);
            var startDate = (long)(context.Range.UtcFrom.ToUnixTimeStamp() * 1000);
            var endDate = (long)(context.Range.UtcTo.ToUnixTimeStamp() * 1000);

            var r = await api.GetCandlestickBars(pairCode, interval, startDate, endDate).ConfigureAwait(false);

            var ohlc = new OhlcData(context.Market);

            var seriesId = OhlcUtilities.GetHash(context.Pair, context.Market, Network);

            foreach (var rEntry in r)
            {
                var dateTime = ((long)(rEntry[0] / 1000)).ToUtcDateTime();
                ohlc.Add(new OhlcEntry(seriesId, dateTime, this)
                {
                    Open = (double)rEntry[1],
                    Close = (double)rEntry[4],
                    Low = (double)rEntry[3],
                    High = (double)rEntry[2],
                    VolumeTo = (double)rEntry[7], // Quote asset volume
                    VolumeFrom = (double)rEntry[5], // Volume
                    WeightedAverage = 0 // BUG: no WeightedAverage data returned from API.
                });
            }

            ohlc.Reverse();

            return ohlc;
        }

        private string ConvertToBinanceInterval(TimeResolution resolution)
        {
            // BUG: API doesn't support seconds and milliseconds.
            switch (resolution)
            {
                case TimeResolution.Minute:
                    return "1m";
                case TimeResolution.Hour:
                    return "1h";
                case TimeResolution.Day:
                    return "1d";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetSymbolPriceTicker().ConfigureAwait(false);

            var lowerPairTicker = context.Pair.TickerSimple().ToLower();

            var lpr = r.FirstOrDefault(x => x.symbol.ToLower().Equals(lowerPairTicker));

            if (lpr == null)
                throw new NoAssetPairException(context.Pair, this);

            return new MarketPrice(context.Pair, lpr.price);
        }

        public Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetSymbolPriceTicker().ConfigureAwait(false);

            var prices = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var lowerPairTicker = pair.TickerSimple().ToLower();

                var lpr = r.FirstOrDefault(x => x.symbol.ToLower().Equals(lowerPairTicker));

                if (lpr == null)
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.MarketPrices.Add(new MarketPrice(pair, lpr.price));
            }

            return prices;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetSymbolPriceTicker().ConfigureAwait(false);

            var assetPairs = new AssetPairs();

            foreach (var rPrice in r.OrderBy(x => x.symbol.Length))
            {
                var pair = GetAssetPair(rPrice.symbol, assetPairs);

                if (pair == null)
                    continue; //throw new ApiResponseException($"Error during {rPrice.symbol} asset pair parsing", this);

                assetPairs.Add(pair);
            }

            return assetPairs;
        }

        private AssetPair GetAssetPair(string pairCode, AssetPairs existingPairs)
        {
            AssetPair assetPair = null;

            if (pairCode.Length == 6)
            {
                var asset1 = pairCode.Substring(0, 3);
                var asset2 = pairCode.Substring(3);

                assetPair = new AssetPair(asset1, asset2);
            }
            else if (pairCode.Length > 6)
            {
                var existingAsset = FindAssetByPairCode(pairCode, existingPairs);

                if (existingAsset == null)
                    return null;

                var asset1 = pairCode.Replace(existingAsset.ShortCode, "");
                var asset2 = existingAsset.ShortCode;

                assetPair = pairCode.StartsWith(existingAsset.ShortCode)
                    ? new AssetPair(asset2, asset1)
                    : new AssetPair(asset1, asset2);
            }

            return assetPair;
        }

        private Asset FindAssetByPairCode(string pairCode, AssetPairs pairs)
        {
            var asset = pairs.FirstOrDefault(x => pairCode.StartsWith(x.Asset1.ShortCode))?.Asset1;
            if (asset != null)
                return asset;

            asset = pairs.FirstOrDefault(x => pairCode.StartsWith(x.Asset2.ShortCode))?.Asset2;
            if (asset != null)
                return asset;

            asset = pairs.FirstOrDefault(x => pairCode.EndsWith(x.Asset1.ShortCode))?.Asset1;
            if (asset != null)
                return asset;

            asset = pairs.FirstOrDefault(x => pairCode.EndsWith(x.Asset2.ShortCode))?.Asset2;
            if (asset != null)
                return asset;

            return null;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            CheckOrderRecordsInputNumber(context.MaxRecordsCount);

            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.TickerSimple();

            var r = context.MaxRecordsCount.HasValue
                ? await api.GetOrderBook(pairCode, context.MaxRecordsCount.Value / 2).ConfigureAwait(false)
                : await api.GetOrderBook(pairCode).ConfigureAwait(false);

            var orderBook = new OrderBook();

            foreach (var rBid in r.asks)
            {
                var bid = GetOrderBookRecordData(rBid);

                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Bid,
                    Data = new BidAskData()
                    {
                        Time = DateTime.UtcNow,
                        Price = new Money(bid.Price, context.Pair.Asset2),
                        Volume = bid.Volume
                    }
                });
            }

            foreach (var rAsk in r.asks)
            {
                var ask = GetOrderBookRecordData(rAsk);

                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Ask,
                    Data = new BidAskData()
                    {
                        Time = DateTime.UtcNow,
                        Price = new Money(ask.Price, context.Pair.Asset2),
                        Volume = ask.Volume
                    }
                });
            }

            return orderBook;
        }

        private (decimal Price, decimal Volume) GetOrderBookRecordData(object[] rawEntry)
        {
            if (!decimal.TryParse(rawEntry[0].ToString(), out var price))
                throw new ApiResponseException("Incorrect format of returned price", this);

            if (!decimal.TryParse(rawEntry[1].ToString(), out var volume))
                throw new ApiResponseException("Incorrect format of returned volume", this);

            return (price, volume);
        }

        private void CheckOrderRecordsInputNumber(int? recordsNumber)
        {
            var validRecordsCount = new int[] { 5, 10, 20, 50, 100, 200, 500 };

            if (recordsNumber.HasValue && !validRecordsCount.Contains(recordsNumber.Value))
                throw new ArgumentException("Specified number of order book records is not supported");
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountInformation().ConfigureAwait(false);

            return r != null;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountInformation().ConfigureAwait(false);

            var balances = new BalanceResults();

            foreach (var b in r.balances)
            {
                var asset = b.asset.ToAsset(this);

                balances.Add(new BalanceResult(asset)
                {
                    Available = new Money(b.free, asset),
                    Balance = new Money(b.free, asset),
                    Reserved = new Money(b.locked, asset)
                });
            }

            return balances;
        }

        public async Task<VolumeResult> GetVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.TickerSimple();

            var r = await api.Get24HrTicker(pairCode).ConfigureAwait(false);

            return new VolumeResult()
            {
                Pair = context.Pair,
                Volume = r.volume,
                Period = VolumePeriod.Day
            };
        }
    }
}
