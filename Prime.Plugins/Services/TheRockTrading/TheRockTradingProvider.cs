using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.TheRockTrading
{
    // https://api.therocktrading.com/doc/v1/
    public class TheRockTradingProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string TheRockTradingApiVersion = "v1";
        private const string TheRockTradingApiUrl = "https://api.therocktrading.com/" + TheRockTradingApiVersion;

        private static readonly ObjectId IdHash = "prime:therocktrading".GetObjectIdHashCode();

        //API calls are limited to 10 requests per second. Do not go over this limit or you will be blacklisted.
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(10, 1); //TODO - only left like this temporarily

        private RestApiClientProvider<ITheRockTradingApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("TheRockTrading");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;

        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public TheRockTradingProvider()
        {
            ApiProvider = new RestApiClientProvider<ITheRockTradingApi>(TheRockTradingApiUrl, this, (k) => null);
        }

        public Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            return Task.Run(() => true);
        }
        
        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.tickers)
            {
                if (rCurrentTicker.fund_id.Length >= 6)
                {
                    //Adds an underscore between the assets.
                    pairs.Add(rCurrentTicker.fund_id.Insert(3, "_").ToAssetPairRaw());
                }
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures(true, false);
        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, "");
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.last));
        }
    }
}
