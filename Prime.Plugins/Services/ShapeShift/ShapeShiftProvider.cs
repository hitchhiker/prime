using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.ShapeShift
{
    public class ShapeShiftProvider : IPublicPricingProvider
    {
        private const string ShapeShiftApiUrl = "https://shapeshift.io";

        private static readonly ObjectId IdHash = "prime:shapeshift".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("ShapeShift");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        private RestApiClientProvider<IShapeShiftApi> ApiProvider { get; }

        public ShapeShiftProvider()
        {
            ApiProvider = new RestApiClientProvider<IShapeShiftApi>(ShapeShiftApiUrl, this, k => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var priceCtx = new PublicPriceContext("BTC_LTC".ToAssetPair(this));

            var r = await GetPriceAsync(priceCtx).ConfigureAwait(false);
            return r != null && r.IsCompleted && r.FirstPrice != null;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures(),
            Bulk = new PricingBulkFeatures() { CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;
        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        private async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetMarketInfo(pairCode).ConfigureAwait(false);
            
            return new MarketPrices(new MarketPrice(Network, context.Pair, r.rate));
        }

        private async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetMarketInfos().ConfigureAwait(false);

            var pairsDict = r.ToDictionary(x => x.pair.ToAssetPair(this), x => x);

            var pairsQueryable = context.IsRequestAll
                ? pairsDict.Keys.ToArray()
                : context.Pairs;

            var prices = new MarketPrices();

            foreach (var pair in pairsQueryable)
            {
                if (!pairsDict.TryGetValue(pair, out var price))
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.Add(new MarketPrice(Network, pair, price.rate));
            }

            return prices;
        }
    }
}
