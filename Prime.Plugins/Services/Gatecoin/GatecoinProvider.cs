using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;
using System.Linq;

namespace Prime.Plugins.Services.Gatecoin
{
    // https://gatecoin.com/api/
    public class GatecoinProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string GatecoinApiUrl = "https://api.gatecoin.com/Public/";

        private static readonly ObjectId IdHash = "prime:gatecoin".GetObjectIdHashCode();

        //Taken from API doc: "There is no rate limit on any public API."
        //https://gatecoin.com/api/
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IGatecoinApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Gatecoin");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public char? CommonPairSeparator { get; }

        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public GatecoinProvider()
        {
            ApiProvider = new RestApiClientProvider<IGatecoinApi>(GatecoinApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync("BTCUSD").ConfigureAwait(false);

            return r?.responseStatus?.message?.Equals("OK", StringComparison.InvariantCultureIgnoreCase) == true;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.responseStatus?.message?.Equals("OK", StringComparison.InvariantCultureIgnoreCase) == false || r.tickers == null || r.tickers.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned.", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.tickers)
            {
                pairs.Add(rCurrentTicker.currencyPair.ToAssetPair(this, 3));
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

            if (r == null || r.responseStatus?.message?.Equals("OK", StringComparison.InvariantCultureIgnoreCase) == false || r.ticker == null)
            {
                throw new ApiResponseException("No asset pairs returned.", this);
            }

            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.ticker.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ticker.ask, r.ticker.bid, r.ticker.low, r.ticker.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.ticker.volume)
            });
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.responseStatus?.message?.Equals("OK", StringComparison.InvariantCultureIgnoreCase) == false || r.tickers == null || r.tickers.Length == 0)
            {
                throw new ApiResponseException("No tickers returned.", this);
            }

            var prices = new MarketPricesResult();

            var rPairsDict = r.tickers.ToDictionary(x => x.currencyPair.ToAssetPair(this, 3), x => x);
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
