using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.LakeBtc
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://www.lakebtc.com/s/api_v2
    public class LakeBtcProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string LakeBtcApiVersion = "v2";
        private const string LakeBtcApiUrl = "https://api.LakeBTC.com/api_" + LakeBtcApiVersion;

        private static readonly ObjectId IdHash = "prime:lakebtc".GetObjectIdHashCode();

        //Data is cached on the server-side so it is unnecessary to make frequent visits.
        //https://www.lakebtc.com/s/api_v2
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<ILakeBtcApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("LakeBTC");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public LakeBtcProvider()
        {
            ApiProvider = new RestApiClientProvider<ILakeBtcApi>(LakeBtcApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r?.Count > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r)
            {
                pairs.Add(rCurrentTicker.Key.ToAssetPair(this, 3));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var prices = new MarketPrices();

            var pairsQueryable = context.IsRequestAll ? r.Keys.Select(x => x.ToAssetPair(this, 3)).ToList() : context.Pairs;

            foreach (var pair in pairsQueryable)
            {
                r.TryGetValue(pair.ToTicker(this).ToLower(), out var currentTicker);

                if (currentTicker == null)
                {
                    prices.MissedPairs.Add(pair);
                }
                else
                {
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.ask, currentTicker.bid, currentTicker.low, currentTicker.high),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.volume)
                    });
                }
            }

            return prices;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = 1000;

            var asks = context.MaxRecordsCount == int.MaxValue ? r.asks.Take(maxCount) : r.asks.Take(context.MaxRecordsCount);
            var bids = context.MaxRecordsCount == int.MaxValue ? r.bids.Take(maxCount) : r.bids.Take(context.MaxRecordsCount);

            foreach (var i in bids.Select(GetBidAskData))
                orderBook.AddBid(i.Item1, i.Item2, true);

            foreach (var i in asks.Select(GetBidAskData))
                orderBook.AddAsk(i.Item1, i.Item2, true);

            return orderBook;
        }

        private Tuple<decimal, decimal> GetBidAskData(decimal[] data)
        {
            decimal price = data[0];
            decimal amount = data[1];

            return new Tuple<decimal, decimal>(price, amount);
        }
    }
}
