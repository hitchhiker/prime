using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.ItBit
{
    // https://api.itbit.com/docs
    public class ItBitProvider : IAssetPairsProvider, IPublicPricingProvider, IAssetPairVolumeProvider
    {
        private readonly string _pairs = "xbtusd,xbtsgd,xbteur";
        private readonly string ItBitApiUrl = "https://api.itbit.com/v1/";

        private static readonly ObjectId IdHash = "prime:itbit".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);

        public Network Network { get; } = new Network("ItBit");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        private RestApiClientProvider<IItBitApi> ApiProvider { get; }

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;

        public ItBitProvider()
        {
            ApiProvider = new RestApiClientProvider<IItBitApi>(ItBitApiUrl, this, k => null);
        }

        private static readonly IAssetCodeConverter AssetCodeConverter = new ItBitCodeConverter();

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var r = await GetAssetPairsAsync(context).ConfigureAwait(false);

            return r.Count > 0;
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanSatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, "");

            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            var price = new MarketPrice(Network, context.Pair, r.lastPrice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low24h, r.high24h),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume24h)
            };

            return new MarketPricesResult(price);
        }

        public async Task<NetworkPairVolume> GetAssetPairVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, ""); 

            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new NetworkPairVolume(Network, context.Pair, r.volume24h);
        }
    }
}
