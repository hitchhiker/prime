using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Coinroom
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://coinroom.com/public-api
    public class CoinroomProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string CoinroomApiUrl = "https://coinroom.com/api/";

        private static readonly ObjectId IdHash = "prime:coinroom".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<ICoinroomApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Coinroom");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '/';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public CoinroomProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinroomApi>(CoinroomApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetCurrenciesAsync().ConfigureAwait(false);

            return r?.crypto?.Length > 0 && r.real?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetCurrenciesAsync().ConfigureAwait(false);

            var pairs = new AssetPairs();

            if (r.crypto.Length <= 0 || r.real.Length <= 0) return pairs;

            foreach (string currentCrypto in r.crypto)
            {
                foreach (string currentReal in r.real)
                {
                    pairs.Add(new AssetPair(currentCrypto, currentReal));
                }
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
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }
    }
}
