using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Btcc
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://www.btcc.com/apidocs/usd-spot-exchange-market-data-rest-api
    public class BtccProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string BtccApiUrl = "https://spotusd-data.btcc.com/data/pro/";

        private static readonly ObjectId IdHash = "prime:btcc".GetObjectIdHashCode();

        //Could not find a rate limiter in official documentation.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBtccApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Btcc");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs() { "BTC_USD".ToAssetPair(this, '_') });

        public BtccProvider()
        {
            ApiProvider = new RestApiClientProvider<IBtccApi>(BtccApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync("BTCUSD").ConfigureAwait(false);

            return r?.ticker != null;
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
            
            return new MarketPrices(new MarketPrice(Network, context.Pair, r.ticker.Last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ticker.AskPrice, r.ticker.BidPrice, r.ticker.Low, r.ticker.High),
                Volume = new NetworkPairVolume(Network, context.Pair, r.ticker.Volume24H)
            });
        }
    }
}
