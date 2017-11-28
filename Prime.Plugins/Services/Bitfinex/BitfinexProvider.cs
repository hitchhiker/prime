using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Bitfinex
{
    // https://bitfinex.readme.io/v1/reference
    public class BitfinexProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string BitfinexApiVersion = "v1";
        private const string BitfinexApiUrl = "https://api.bitfinex.com/" + BitfinexApiVersion;

        private static readonly ObjectId IdHash = "prime:bitfinex".GetObjectIdHashCode();

        //If an IP address exceeds a certain number of requests per minute (between 10 and 90) to a specific REST API endpoint e.g., /ticker, the requesting IP address will be blocked for 10-60 seconds on that endpoint and the JSON response {"error": "ERR_RATE_LIMIT"} will be returned. 
        //Please note the exact logic and handling for such DDoS defenses may change over time to further improve reliability.
        //https://bitfinex.readme.io/v1/docs
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(10, 1);

        private RestApiClientProvider<IBitfinexApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Bitfinex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator { get; }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BitfinexProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitfinexApi>(BitfinexApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAssetsAsync().ConfigureAwait(false);

            return r?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetsAsync().ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned.", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentAssetPair in r)
            {
                pairs.Add(rCurrentAssetPair.ToAssetPair(this, 3));
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

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, "");
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.last_price)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }
    }
}
