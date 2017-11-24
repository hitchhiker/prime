using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Exmo
{
    // https://exmo.com/en/api#/public_api
    public class ExmoProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string ExmoApiVersion = "v1";
        private const string ExmoApiUrl = "https://api.exmo.com/" + ExmoApiVersion;

        private static readonly ObjectId IdHash = "prime:exmo".GetObjectIdHashCode();

        //The number of API requests is limited to 180 per/minute from one IP address or by a single user.
        //https://exmo.com/en/api#/public_api
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(180, 1);

        private RestApiClientProvider<IExmoApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Exmo");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator { get; }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public ExmoProvider()
        {
            ApiProvider = new RestApiClientProvider<IExmoApi>(ExmoApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetCurrencyAsync().ConfigureAwait(false);

            return r?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned.", this);
            }

            var pairs = new AssetPairs();

            foreach (var entry in r)
            {
                pairs.Add(entry.Key.ToAssetPair(this, '_'));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No tickers returned.", this);
            }

            var prices = new MarketPricesResult();
            
            var rPairsDict = r.ToDictionary(x => x.Key.ToAssetPair(this), x => x.Value);

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
                    prices.MarketPrices.Add(new MarketPrice(Network, pair, currentTicker.last_trade)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.sell_price, currentTicker.buy_price, currentTicker.low, currentTicker.high),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.vol)
                    });
                }
            }

            return prices;
        }
    }
}
