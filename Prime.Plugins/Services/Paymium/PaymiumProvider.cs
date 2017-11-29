using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Paymium
{
    // https://github.com/Paymium/api-documentation
    public class PaymiumProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string PaymiumApiVersion = "v1";
        private const string PaymiumApiUrl = "https://paymium.com/api/" + PaymiumApiVersion;

        private static readonly ObjectId IdHash = "prime:paymium".GetObjectIdHashCode();

        //API calls are rate-limited by IP to 86400 calls per day
        //https://github.com/Paymium/api-documentation#rate-limiting
        private static readonly IRateLimiter Limiter = new PerDayRateLimiter(86400,1);

        private RestApiClientProvider<IPaymiumApi> ApiProvider { get; }

        private const string PairsCsv = "BTCEUR,EURBTC";

        public Network Network { get; } = Networks.I.Get("Paymium");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public char? CommonPairSeparator => null;

        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public PaymiumProvider()
        {
            ApiProvider = new RestApiClientProvider<IPaymiumApi>(PaymiumApiUrl, this, (k) => null);
        }

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetCountriesAsync().ConfigureAwait(false);

            return r?.Length > 0;
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

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var currencyCode = context.Pair.Asset1.ToRemoteCode(this);
            var r = await api.GetTickerAsync(currencyCode).ConfigureAwait(false);
            
            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.price)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }
    }
}
