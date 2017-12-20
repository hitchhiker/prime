using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.IndependentReserve
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://www.independentreserve.com/API
    public class IndependentReserveProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string IndependentReserveApiUrl = "https://api.independentreserve.com/Public/";

        private static readonly ObjectId IdHash = "prime:independentreserve".GetObjectIdHashCode();

        //The API is rate limited to 1 call per second.
        //https://www.independentreserve.com/API
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(1, 1);

        private RestApiClientProvider<IIndependentReserveApi> ApiProvider { get; }

        private static readonly IAssetCodeConverter AssetCodeConverter = new IndependentReserveCodeConverter();

        public Network Network { get; } = Networks.I.Get("IndependentReserve");

        private const string PairsCsv = "btcusd,btcaud,btcnzd,ethusd,ethaud,ethnzd,bchusd,bchaud,bchnzd";

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public IndependentReserveProvider()
        {
            ApiProvider = new RestApiClientProvider<IIndependentReserveApi>(IndependentReserveApiUrl, this, (k) => null);
        }

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync("xbt", "usd").ConfigureAwait(false);

            return r?.LastPrice > 0;
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            string pairCode = context.Pair.ToTicker(this);
            var pairs = pairCode.Split('_');

            if (pairs == null || pairs.Length < 2)
            {
                throw new ApiResponseException("Invalid asset pair passed as parameter");
            }

            var r = await api.GetTickerAsync(pairs[0].ToLower(), pairs[1].ToLower()).ConfigureAwait(false);

            if (r == null)
            {
                throw new ApiResponseException("No ticker found");
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.LastPrice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.CurrentLowestOfferPrice, r.CurrentHighestBidPrice, r.DayLowestPrice, r.DayHighestPrice),
                Volume = new NetworkPairVolume(Network, context.Pair, r.DayVolumeXbt, r.DayVolumeXbtInSecondaryCurrrency)
            });
        }
    }
}
