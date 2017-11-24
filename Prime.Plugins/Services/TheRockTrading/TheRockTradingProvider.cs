using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Api.Request.RateLimits;
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
        //https://api.therocktrading.com/doc/v1/
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(10, 1);

        private RestApiClientProvider<ITheRockTradingApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("TheRockTrading");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator { get; }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public TheRockTradingProvider()
        {
            ApiProvider = new RestApiClientProvider<ITheRockTradingApi>(TheRockTradingApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r?.tickers?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r?.tickers == null || r.tickers.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned.", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.tickers)
            {
                pairs.Add(rCurrentTicker.fund_id.ToAssetPair(this, 3));
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

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        public async Task<MarketPricesResult> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, "");
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r == null)
            {
                throw new ApiResponseException("No ticker returned.", this);
            }

            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r?.tickers == null || r.tickers.Length == 0)
            {
                throw new ApiResponseException("No tickers returned.", this);
            }

            var prices = new MarketPricesResult();

            var pairsQueryable = context.IsRequestAll ? r.tickers.Select(x => x.fund_id.ToAssetPair(this, 3)) : context.Pairs;

            foreach (var pair in pairsQueryable)
            {
                var currentTicker = r.tickers.FirstOrDefault(x => x.fund_id.ToAssetPair(this, 3).Equals(pair));

                if (currentTicker == null)
                {
                    prices.MissedPairs.Add(pair);
                }
                else
                {
                    prices.MarketPrices.Add(new MarketPrice(Network, pair, currentTicker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.ask, currentTicker.bid, currentTicker.low, currentTicker.high),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.volume)
                    });
                }
            }

            return prices;
        }
    }
}
