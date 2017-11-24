using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Api.Request.RateLimits;
using Prime.Utility;

namespace Prime.Plugins.Services.Tidex
{
    // https://tidex.com/public-api
    public class TidexProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string TidexApiVersion = "3";
        private const string TidexApiUrl = "https://api.tidex.com/api/" + TidexApiVersion;

        private static readonly ObjectId IdHash = "prime:tidex".GetObjectIdHashCode();

        //From doc: All information is cached every 2 seconds, so there's no point in making more frequent requests.
        //https://tidex.com/public-api
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(1, 2);

        private RestApiClientProvider<ITidexApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Tidex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator { get; }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public TidexProvider()
        {
            ApiProvider = new RestApiClientProvider<ITidexApi>(TidexApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            return r?.pairs?.Count > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            if (r == null || r.pairs.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned.", this);
            }

            var pairs = new AssetPairs();

            foreach (string entry in r.pairs.Keys)
            {
                pairs.Add(entry.ToAssetPair(this, '_'));
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
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);

        }

        public async Task<MarketPricesResult> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairsCsv = string.Join("-", context.Pairs.Select(x => x.ToTicker(this, '_').ToLower()));
            var r = await api.GetTickerAsync(pairsCsv).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No tickers returned.", this);
            }

            var ticker = r.FirstOrDefault().Value;

            if (ticker != null)
            {
                return new MarketPricesResult(new MarketPrice(Network, context.Pair, ticker.last)
                {
                    PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, ticker.sell, ticker.buy, ticker.low, ticker.high),
                    Volume = new NetworkPairVolume(Network, context.Pair, ticker.vol)
                });
            }
            else
            {
                throw new ApiResponseException("No data returned", this);
            }
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairsCsv = string.Join("-", context.Pairs.Select(x => x.ToTicker(this, '_').ToLower()));
            var r = await api.GetTickerAsync(pairsCsv).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No tickers returned.", this);
            }

            var prices = new MarketPricesResult();
            
            foreach (var pair in context.Pairs)
            {
                var currentTicker = r.FirstOrDefault(x => x.Key.ToAssetPair(this, '_').Equals(pair)).Value;

                if (currentTicker == null)
                {
                    prices.MissedPairs.Add(pair);
                }
                else
                {
                    prices.MarketPrices.Add(new MarketPrice(Network, pair, currentTicker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.sell, currentTicker.buy, currentTicker.low, currentTicker.high),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.vol)
                    });
                }
            }

            return prices;
        }
    }
}
