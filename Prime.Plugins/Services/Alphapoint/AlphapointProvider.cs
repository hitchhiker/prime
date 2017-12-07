using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Alphapoint
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://alphapoint.com/api/docs/public/
    public class AlphapointProvider : /*PricingProvider,*/ IAssetPairsProvider
    {
        private const string AlphapointApiVersion = "v1";
        private const string AlphapointApiUrl = "https://sim3.alphapoint.com:8400/ajax/" + AlphapointApiVersion;

        private static readonly ObjectId IdHash = "prime:alphapoint".GetObjectIdHashCode();

        //More than 500 request per 10 minutes will result in IP ban.
        //https://alphapoint.com/api/docs/public/
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(500, 10);

        private RestApiClientProvider<IAlphapointApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Alphapoint");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public AlphapointProvider()
        {
            ApiProvider = new RestApiClientProvider<IAlphapointApi>(AlphapointApiUrl, this, (k) => null);
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
    }
}
