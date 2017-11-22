using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Gdax
{
    // https://docs.gdax.com/
    public class GdaxProvider : IAssetPairsProvider, IPublicPricingProvider
    {
        private const string GdaxApiUrl = "https://api.gdax.com";

        private static readonly ObjectId IdHash = "prime:gdax".GetObjectIdHashCode();

        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("GDAX");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        /// <summary>
        /// We throttle public endpoints by IP: 3 requests per second, up to 6 requests per second in bursts.
        /// We throttle private endpoints by user ID: 5 requests per second, up to 10 requests per second in bursts.
        /// See https://docs.gdax.com/#rate-limits.
        /// </summary>
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(180, 1, 300, 1);

        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public string CommonPairSeparator => "-";

        private RestApiClientProvider<IGdaxApi> ApiProvider { get; }

        public GdaxProvider()
        {
            ApiProvider = new RestApiClientProvider<IGdaxApi>(GdaxApiUrl, this, k => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetCurrentServerTimeAsync().ConfigureAwait(false);

            return r != null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetProductsAsync().ConfigureAwait(false);

            var pairs = new AssetPairs();

            foreach (var rProduct in r)
            {
                pairs.Add(new AssetPair(rProduct.base_currency, rProduct.quote_currency));
            }

            return pairs;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures {Single = new PricingSingleFeatures{CanStatistics = true, CanVolume = true}};
        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetProductTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.price)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetProductTickerAsync(pairCode).ConfigureAwait(false);

            return new PublicVolumeResponse(Network, context.Pair, r.volume);
        }

        public VolumeFeatures VolumeFeatures { get; }
    }
}
