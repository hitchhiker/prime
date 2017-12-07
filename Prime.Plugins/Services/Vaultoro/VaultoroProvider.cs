using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Vaultoro
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://api.vaultoro.com/#api-Basic_API
    public class VaultoroProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string VaultoroApiUrl = "https://api.vaultoro.com/";

        private static readonly ObjectId IdHash = "prime:vaultoro".GetObjectIdHashCode();

        //The calls are limited to 1-2 per second. We will be testing different settings when load increases.
        //https://api.vaultoro.com/#api-Basic_API
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(2, 1);

        private RestApiClientProvider<IVaultoroApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Vaultoro");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '-';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public VaultoroProvider()
        {
            ApiProvider = new RestApiClientProvider<IVaultoroApi>(VaultoroApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetMarketsAsync().ConfigureAwait(false);

            return r?.status.Equals("success", StringComparison.InvariantCultureIgnoreCase) == true;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetMarketsAsync().ConfigureAwait(false);

            if (r.status.Equals("success", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs
            {
                r.data.MarketName.ToAssetPair(this)
            };

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
            var r = await api.GetMarketsAsync().ConfigureAwait(false);

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.data.LastPrice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, null,null, r.data.Low24h, r.data.High24h),
                Volume = new NetworkPairVolume(Network, context.Pair, r.data.Volume24h)
            });
        }
    }
}
