using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Whaleclub
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public class WhaleclubProvider : IAssetPairsProvider, IPublicPricingProvider
    {
        private const string WhaleclubApiVersion = "v1";
        private const string WhaleclubApiUrl = "https://api.whaleclub.co/" + WhaleclubApiVersion;

        private static readonly ObjectId IdHash = "prime:whaleclub".GetObjectIdHashCode();

        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("Whaleclub");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        public char? CommonPairSeparator => '-';

        private RestApiClientProvider<IWhaleclubApi> ApiProvider { get; }

        // By default, each API token is rate limited at 60 requests per minute.
        // Additionally, requests are throttled up to a maximum of 20 requests per second.
        private static IRateLimiter Limiter = new PerSecondRateLimiter(1, 1);
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;

        public WhaleclubProvider()
        {
            ApiProvider = new RestApiClientProvider<IWhaleclubApi>(WhaleclubApiUrl, this, k => new WhaleclubAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var privateCtx = new NetworkProviderPrivateContext(UserContext.Current);
            var pairs = await GetAssetPairsAsync(privateCtx).ConfigureAwait(false);

            return pairs?.Count > 0;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var ctxPrivate = new NetworkProviderPrivateContext(UserContext.Current);

            var api = ApiProvider.GetApi(ctxPrivate);
            var r = await api.GetMarkets().ConfigureAwait(false);

            var rFiltered = r.Where(x =>
                    x.Value.category.Equals("crypto", StringComparison.OrdinalIgnoreCase) ||
                    x.Value.category.Equals("forex", StringComparison.OrdinalIgnoreCase));

            var pairs = new AssetPairs();
            foreach (var pair in rFiltered)
            {
                pairs.Add(pair.Key.ToAssetPair(this));
            }

            return pairs;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Bulk = new PricingBulkFeatures() { CanStatistics = true },
            Single = new PricingSingleFeatures() { CanStatistics = true }
        };
        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        private async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var privateCtx = new NetworkProviderPrivateContext(UserContext.Current);
            var api = ApiProvider.GetApi(privateCtx);

            var pairsCsv = string.Join(",", context.Pairs.Select(x => x.ToTicker(this)));

            var r = await api.GetPrices(pairsCsv).ConfigureAwait(false);

            var prices = new MarketPrices();

            foreach (var pair in context.Pairs)
            {
                if (!r.TryGetValue(pair.ToTicker(this), out var price))
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.Add(new MarketPrice(Network, pair, (price.ask + price.bid) / 2)
                {
                    PriceStatistics = new PriceStatistics(Network, pair.Asset2, price.ask, price.bid)
                });
            }

            return prices;
        }

        private async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var privateCtx = new NetworkProviderPrivateContext(UserContext.Current);

            var api = ApiProvider.GetApi(privateCtx);
            var code = context.Pair.ToTicker(this);

            var r = await api.GetPrices(code).ConfigureAwait(false);

            if (!r.TryGetValue(context.Pair.ToTicker(this), out var price))
                throw new AssetPairNotSupportedException(context.Pair, this);

            return new MarketPrices(new MarketPrice(Network, context.Pair, (price.ask + price.bid) / 2)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, price.ask, price.bid)
            });
        }
    }
}
