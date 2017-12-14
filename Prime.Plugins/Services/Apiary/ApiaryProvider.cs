using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Apiary
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://dsx.docs.apiary.io/#
    public class ApiaryProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string ApiaryApiUrl = "https://dsx.uk/mapi/" ;

        private static readonly ObjectId IdHash = "prime:apiary".GetObjectIdHashCode();

        //If you make more than 60 requests of Trading API methods (/tapi/) per minute using the same API Keys you will receive an error on each extra request during the same minute. Each new minute the counter becomes 0.
        //https://dsx.docs.apiary.io/#introduction/requests-limit
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(60, 1);

        private RestApiClientProvider<IApiaryApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Apiary");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public ApiaryProvider()
        {
            ApiProvider = new RestApiClientProvider<IApiaryApi>(ApiaryApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            return r.pairs?.Count > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            if (r?.pairs == null || r.pairs.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.pairs)
            {
                pairs.Add(rCurrentTicker.Key.ToAssetPair(this,3));
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
            var pairCode = context.Pair.ToTicker(this).ToLower();
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No ticker returned", this);
            }

            r.TryGetValue(pairCode, out var currentTicker);

            if (currentTicker == null)
            {
                throw new ApiResponseException("No ticker returned", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, currentTicker.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, currentTicker.sell, currentTicker.buy, currentTicker.low, currentTicker.high),
                Volume = new NetworkPairVolume(Network, context.Pair, currentTicker.vol)
            });
        }
    }
}
