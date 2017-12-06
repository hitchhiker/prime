using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Tidex
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://tidex.com/public-api
    public class TidexProvider : IPublicPricingProvider, IAssetPairsProvider, INetworkProviderPrivate
    {
        private const string TidexApiVersion = "3";
        private const string TidexApiUrlPublic = "https://api.tidex.com/api/" + TidexApiVersion;
        private const string TidexApiUrlPrivate = "https://api.tidex.com/tapi";

        private static readonly ObjectId IdHash = "prime:tidex".GetObjectIdHashCode();

        //From doc: All information is cached every 2 seconds, so there's no point in making more frequent requests.
        //https://tidex.com/public-api
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(1, 2);

        public Network Network { get; } = Networks.I.Get("Tidex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;


        private Dictionary<string, object> CreateTidexPostBody()
        {
            return new Dictionary<string, object>();
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreateTidexPostBody();
            body.Add("method", "getInfo");

            var r = await api.GetUserInfoAsync(body).ConfigureAwait(false);

            CheckResponse(r);

            return r.return_ != null;
        }

        private void CheckResponse<T>(TidexSchema.BaseResponse<T> r)
        {
            if(r.success != 1)
                throw new ApiResponseException(r.error, this);
        }

        private RestApiClientProvider<ITidexApi> ApiProviderPublic { get; }
        private RestApiClientProvider<ITidexApi> ApiProviderPrivate { get; }

        public TidexProvider()
        {
            ApiProviderPublic = new RestApiClientProvider<ITidexApi>(TidexApiUrlPublic);
            ApiProviderPrivate = new RestApiClientProvider<ITidexApi>(TidexApiUrlPrivate, this, (k) => new TidexAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProviderPublic.GetApi(context);
            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            return r?.pairs?.Count > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProviderPublic.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            if (r == null || r.pairs.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (string entry in r.pairs.Keys)
            {
                pairs.Add(entry.ToAssetPair(this));
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

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);

        }

        public async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProviderPublic.GetApi(context);
            var pairsCsv = string.Join("-", context.Pairs.Select(x => x.ToTicker(this).ToLower()));
            var r = await api.GetTickerAsync(pairsCsv).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var ticker = r.FirstOrDefault().Value;

            if (ticker != null)
            {
                return new MarketPrices(new MarketPrice(Network, context.Pair, ticker.last)
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

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProviderPublic.GetApi(context);
            var pairsCsv = string.Join("-", context.Pairs.Select(x => x.ToTicker(this).ToLower()));
            var r = await api.GetTickerAsync(pairsCsv).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var prices = new MarketPrices();

            foreach (var pair in context.Pairs)
            {
                var currentTicker = r.FirstOrDefault(x => x.Key.ToAssetPair(this).Equals(pair)).Value;

                if (currentTicker == null)
                {
                    prices.MissedPairs.Add(pair);
                }
                else
                {
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last)
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
