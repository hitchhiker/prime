using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Coinsecure
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://api.coinsecure.in/v1/slateUI
    public class CoinsecureProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string CoinsecureApiVersion = "v1";
        private const string CoinsecureApiUrl = "https://api.coinsecure.in/" + CoinsecureApiVersion;

        private static readonly ObjectId IdHash = "prime:coinsecure".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<ICoinsecureApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Coinsecure");
        private const string PairsCsv = "BTCINR";

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public CoinsecureProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinsecureApi>(CoinsecureApiUrl, this, (k) => null);
        }

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r?.success == true && r.message != null;
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
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
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.success == false || r.message == null)
            {
                throw new ApiResponseException("No tickers found", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.message.lastPrice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.message.ask, r.message.bid, r.message.low, r.message.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.message.coinvolume, r.message.fiatvolume)
            });
        }
    }
}
