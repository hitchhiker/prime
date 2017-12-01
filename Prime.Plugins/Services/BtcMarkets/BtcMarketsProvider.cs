using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BtcMarkets
{
    // https://github.com/BTCMarkets/API/wiki/Market-data-API
    public class BtcMarketsProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string BtcMarketsApiUrl = "https://api.btcmarkets.net/";

        private static readonly ObjectId IdHash = "prime:btcmarkets".GetObjectIdHashCode();
        private const string PairsCsv = "bchaud,btcaud,ethaud,ltcaud,xrpaud,etcaud";

        // From doc: "Default limit: 25 calls per 10 seconds"
        // https://github.com/BTCMarkets/API/wiki/faq
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(25, 10);

        private RestApiClientProvider<IBtcMarketsApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("BtcMarkets");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '/';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public BtcMarketsProvider()
        {
            ApiProvider = new RestApiClientProvider<IBtcMarketsApi>(BtcMarketsApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("BTC/AUD".ToAssetPair(this));
            var r = await GetPricingAsync(ctx).ConfigureAwait(false);

            return r != null;
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
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);
            
            return new MarketPrices(new MarketPrice(Network, context.Pair, r.lastPrice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.bestAsk, r.bestBid),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume24h)
            });
        }
    }
}
