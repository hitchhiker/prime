using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BrightonPeak
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://www.brightonpeak.com/publicApi
    public class BrightonPeakProvider : /*IPublicPricingProvider,*/ IAssetPairsProvider, IOrderBookProvider
    {
        private const string BrightonPeakApiVersion = "v1";
        private const string BrightonPeakApiUrl = "https://api.brightonpeak.com:8400/ajax/" + BrightonPeakApiVersion;

        private static readonly ObjectId IdHash = "prime:brightonpeak".GetObjectIdHashCode();

        //More than 500 request per 10 minutes will result in IP ban.
        //https://www.brightonpeak.com/publicApi
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(500, 10);

        private RestApiClientProvider<IBrightonPeakApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("BrightonPeak");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BrightonPeakProvider()
        {
            ApiProvider = new RestApiClientProvider<IBrightonPeakApi>(BrightonPeakApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            return r?.productPairs?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            if (r?.productPairs == null || r.productPairs?.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.productPairs)
            {
                pairs.Add(rCurrentTicker.name.ToAssetPair(this, 3));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var body = new Dictionary<string, object>
            {
                { "productPair", pairCode }
            };

            var r = await api.GetTickerAsync(body).ConfigureAwait(false);

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var body = new Dictionary<string, object>
            {
                { "productPair", pairCode }
            };

            var r = await api.GetOrderBookAsync(body).ConfigureAwait(false);

            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var asks = r.asks.Take(maxCount);
            var bids = r.bids.Take(maxCount);

            foreach (var i in bids)
                orderBook.AddBid(i.px, i.qty, true);

            foreach (var i in asks)
                orderBook.AddAsk(i.px, i.qty, true);

            return orderBook;
        }
    }
}
