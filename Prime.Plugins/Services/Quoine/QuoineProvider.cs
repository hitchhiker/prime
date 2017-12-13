using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Quoine
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://developers.quoine.com/#introduction
    public class QuoineProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string QuoineApiUrl = "https://api.quoine.com/";

        private static readonly ObjectId IdHash = "prime:quoine".GetObjectIdHashCode();

        //API users should not make more than 300 requests per 5 minute. Requests go beyond the limit will return with a 429 status
        //https://developers.quoine.com/#rate-limiting
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(300, 5);

        private RestApiClientProvider<IQuoineApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Quoine");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public QuoineProvider()
        {
            ApiProvider = new RestApiClientProvider<IQuoineApi>(QuoineApiUrl, this, (k) => null);
        }

        private ConcurrentDictionary<AssetPair, int> _dictionaryProducts;
        public ConcurrentDictionary<AssetPair, int> PairCodeToProductId => _dictionaryProducts ?? (_dictionaryProducts = PopulateProductsDictionary());

        public ConcurrentDictionary<AssetPair, int> PopulateProductsDictionary()
        {
            var products = GetAllProductsAsync().Result;

            var result = products.ToDictionary(x => x.currency_pair_code.ToAssetPair(this,3), x => x.id);

            return new ConcurrentDictionary<AssetPair, int>(result);
        }

        private async Task<List<QuoineSchema.ProductResponse>> GetAllProductsAsync()
        {
            var api = ApiProvider.GetApi();

            var r = await api.GetProductsAsync().ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No products returned", this);
            }

            var products = r.Where(x => x.product_type.Equals("CurrencyPair", StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (products == null || products.Any() == false)
            {
                throw new ApiResponseException("No products with currency pairs returned", this);
            }

            return products;
        }
        
        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetProductsAsync().ConfigureAwait(false);

            return r?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var products = await GetAllProductsAsync();

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in products)
            {
                pairs.Add(rCurrentTicker.currency_pair_code.ToAssetPair(this,3));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        public async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            PairCodeToProductId.TryGetValue(context.Pair, out var productId);

            if (productId == 0)
            {
                throw new ApiResponseException("No product found with specified asset pair", this);
            }

            var api = ApiProvider.GetApi(context);
            var r = await api.GetProductAsync(productId).ConfigureAwait(false);

            return new MarketPrices(new MarketPrice(Network, context.Pair, (decimal?)r.last_traded_price ?? 0)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, (decimal)r.high_market_ask, (decimal)r.low_market_bid),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume_24h)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var products = await GetAllProductsAsync();

            var prices = new MarketPrices();

            var rPairsDict = products.ToDictionary(x => x.currency_pair_code.ToAssetPair(this,3), x => x);
            var pairsQueryable = context.IsRequestAll ? rPairsDict.Keys.ToList() : context.Pairs;

            foreach (var pair in pairsQueryable)
            {
                rPairsDict.TryGetValue(pair, out var currentTicker);

                if (currentTicker == null)
                {
                    prices.MissedPairs.Add(pair);
                }
                else
                {
                    prices.Add(new MarketPrice(Network, pair, (decimal?)currentTicker.last_traded_price ?? 0)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, (decimal)currentTicker.high_market_ask, (decimal)currentTicker.low_market_bid),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.volume_24h)
                    });
                }
            }

            return prices;
        }
    }
}
