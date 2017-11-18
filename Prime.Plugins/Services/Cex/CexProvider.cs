using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Cex
{
    // https://cex.io/rest-api#public
    public class CexProvider : IPublicPricingProvider, IAssetPairsProvider, IAssetPairVolumeProvider
    {
        private const string CexApiUrl = "https://cex.io/api";

        private static readonly ObjectId IdHash = "prime:cex".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("Cexio");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;

        private RestApiClientProvider<ICexApi> ApiProvider { get; }

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public CexProvider()
        {
            ApiProvider = new RestApiClientProvider<ICexApi>(CexApiUrl, this, k => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var r = await GetAssetPairsAsync(context).ConfigureAwait(false);

            return r.Count > 0;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            var pairs = new AssetPairs();

            foreach (var rTicker in r.data)
            {
                pairs.Add(rTicker.pair.ToAssetPair(this, ':'));
            }

            return pairs;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures(true, true);
        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, "/");

            var r = await api.GetLastPriceAsync(pairCode).ConfigureAwait(false);

            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.lprice));
        }
        
        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            var api = ApiProvider.GetApi(context);
            var r = await api.GetLastPricesAsync().ConfigureAwait(false);
            CheckResponseError(r);

            var prices = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var rPair = r.data.FirstOrDefault(x =>
                    x.symbol1.ToAsset(this).Equals(pair.Asset1) && 
                    x.symbol2.ToAsset(this).Equals(pair.Asset2));

                if (rPair == null)
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.MarketPrices.Add(new MarketPrice(Network, pair, rPair.lprice));
            }

            return prices;
        }

        private void CheckResponseError(CexSchema.BaseResponse response)
        {
            if(!response.ok.Equals("ok")) 
                throw new ApiResponseException($"Error occurred in provider: {response.ok}", this);
        }

        public async Task<NetworkPairVolume> GetAssetPairVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            CheckResponseError(r);

            var rTicker = r.data.FirstOrDefault(x => x.pair.ToAssetPair(this, ':').Equals(context.Pair));

            if(rTicker == null)
                throw new NoAssetPairException(context.Pair, this);

            return new NetworkPairVolume(Network, context.Pair, rTicker.volume);
        }
    }
}
