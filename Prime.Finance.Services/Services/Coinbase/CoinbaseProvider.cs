using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;
using Prime.Finance.Services.Services.Gdax;
using OrderBook = Prime.Core.OrderBook;

namespace Prime.Finance.Services.Services.Coinbase
{
    // https://developers.coinbase.com/api/v2
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class CoinbaseProvider : IOrderBookProvider, IOhlcProvider, IPublicPricingProvider, IAssetPairsProvider
    {
        private static readonly ObjectId IdHash = "prime:coinbase".GetObjectIdHashCode();

        private const string CoinbaseApiVersion = "v2";
        private const string CoinbaseApiUrl = "https://api.coinbase.com/" + CoinbaseApiVersion;

        private const string GdaxApiUrl = "https://api.gdax.com";

        private RestApiClientProvider<ICoinbaseApi> ApiProvider { get; }
        private RestApiClientProvider<IGdaxApi> GdaxApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Coinbase");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '-';

        // 10000 per hour.
        // https://developers.coinbase.com/api/v2#rate-limiting
        private static readonly IRateLimiter Limiter = new PerHourRateLimiter(10000, 1);
        public IRateLimiter RateLimiter => Limiter;
        
        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => true;
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public CoinbaseProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinbaseApi>(CoinbaseApiUrl, this, k => new CoinbaseAuthenticator(k).GetRequestModifierAsync);
            GdaxApiProvider = new RestApiClientProvider<IGdaxApi>(GdaxApiUrl);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetCurrentServerTimeAsync().ConfigureAwait(false);

            return r != null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures(true, false);
        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetLatestPriceAsync(pairCode).ConfigureAwait(false);

            var price = new MarketPrice(Network, context.Pair, r.data.amount);

            return new MarketPrices(price);
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = GdaxApiProvider.GetApi(context);
            var r = await api.GetProductsAsync().ConfigureAwait(false);

            var pairs = new AssetPairs();

            foreach (var rProduct in r)
            {
                pairs.Add(new AssetPair(rProduct.base_currency, rProduct.quote_currency));
            }

            return pairs;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountsAsync().ConfigureAwait(false);
            return r != null;
        }

        public IAssetCodeConverter GetAssetCodeConverter() => null;

        public Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context) => Task.FromResult<TransferSuspensions>(null);

        private Asset FromNetwork(string network)
        {
            switch (network)
            {
                case "bitcoin":
                    return "BTC".ToAssetRaw();
                case "litecoin":
                    return "LTC".ToAssetRaw();
                case "ethereum":
                    return "ETH".ToAssetRaw();
                default:
                    return Asset.None;
            }
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = GdaxApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var r = await api.GetProductOrderBookAsync(pairCode, OrderBookDepthLevel.FullNonAggregated).ConfigureAwait(false);

            var bids = context.MaxRecordsCount == Int32.MaxValue 
                ? r.bids.ToArray()
                : r.bids.Take(context.MaxRecordsCount).ToArray() ;
            var asks = context.MaxRecordsCount == Int32.MaxValue
                ? r.asks.ToArray()
                : r.asks.Take(context.MaxRecordsCount).ToArray();

            var orderBook = new OrderBook(Network, context.Pair);

            foreach (var i in bids.Select(ConvertToOrderBookRecord))
                orderBook.AddBid(i.Price, i.Size);

            foreach (var i in asks.Select(ConvertToOrderBookRecord))
                orderBook.AddAsk(i.Price, i.Size);

            return orderBook;
        }

        private (decimal Price, decimal Size) ConvertToOrderBookRecord(string[] data)
        {
            if(!decimal.TryParse(data[0], out var price) || !decimal.TryParse(data[1], out var size))
                throw new ApiResponseException("API returned incorrect format of price data", this);

            return (price, size);
        }

        public async Task<OhlcDataResponse> GetOhlcAsync(OhlcContext context)
        {
            var api = GdaxApiProvider.GetApi(context);
            var currencyCode = context.Pair.ToTicker(this);

            var ohlc = new OhlcDataResponse(context.Resolution);
            var seriesId = OhlcUtilities.GetHash(context.Pair, context.Resolution, Network);

            var granularitySeconds = GetSeconds(context.Resolution);
            var maxNumberOfCandles = 200;

            var tsFrom = (long)context.Range.UtcFrom.ToUnixTimeStamp();
            var tsTo = (long)context.Range.UtcTo.ToUnixTimeStamp();
            var tsStep = maxNumberOfCandles * granularitySeconds;

            var currTsTo = tsTo;
            var currTsFrom = tsTo - tsStep;

            while (currTsTo > tsFrom)
            {
                var candles = await api.GetCandlesAsync(currencyCode, currTsFrom.ToUtcDateTime(), currTsTo.ToUtcDateTime(), granularitySeconds).ConfigureAwait(false);

                foreach (var candle in candles)
                {
                    var dateTime = ((long)candle[0]).ToUtcDateTime();
                    ohlc.Add(new OhlcEntry(seriesId, dateTime, this)
                    {
                        Low = candle[1],
                        High = candle[2],
                        Open = candle[3],
                        Close = candle[4],
                        VolumeTo = candle[5],
                        VolumeFrom = candle[5],
                        WeightedAverage = 0 // Is not provided by API.
                    });
                }

                currTsTo = currTsFrom;

                if (currTsTo - tsStep >= tsFrom)
                    currTsFrom -= tsStep;
                else
                    currTsFrom = tsFrom;

                ApiHelpers.EnterRate(this, context);
            }

            return ohlc;
        }

        private int GetSeconds(TimeResolution market)
        {
            switch (market)
            {
                case TimeResolution.Second:
                    return 1;
                case TimeResolution.Minute:
                    return 60;
                case TimeResolution.Hour:
                    return 3600;
                case TimeResolution.Day:
                    return 62400;
                default:
                    throw new ArgumentOutOfRangeException(nameof(market), market, null);
            }
        }
    }
}