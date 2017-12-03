using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;
using System.Linq;

namespace Prime.Plugins.Services.NovaExchange
{
    // https://novaexchange.com/remote/faq/
    public class NovaExchangeProvider // : IPublicPricingProvider, IAssetPairsProvider //DISABLED DUE TO RIDICULOUS RATE LIMIT ON MARKET API.
    {
        /*
        private const string NovaExchangeApiVersion = "v2";
        private const string NovaExchangeApiUrl = "https://novaexchange.com/remote/" + NovaExchangeApiVersion;

        private static readonly ObjectId IdHash = "prime:novaexchange".GetObjectIdHashCode();

        //Limited to 1 request per minute
        //https://novaexchange.com/remote/faq/
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(1, 1);

        private RestApiClientProvider<INovaExchangeApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("NovaExchange");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public NovaExchangeProvider()
        {
            ApiProvider = new RestApiClientProvider<INovaExchangeApi>(NovaExchangeApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r.status?.Equals("success", StringComparison.InvariantCultureIgnoreCase) == true;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r.status?.Equals("success", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new ApiResponseException(r.message, this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.markets)
            {
                pairs.Add(rCurrentTicker.marketname.ToAssetPair(this));
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
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r.status?.Equals("success", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new ApiResponseException(r.message, this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.markets[0].last_price)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.markets[0].ask, r.markets[0].bid, r.markets[0].low24h, r.markets[0].high24h),
                Volume = new NetworkPairVolume(Network, context.Pair, r.markets[0].volume24h)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r.status?.Equals("success", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new ApiResponseException(r.message, this);
            }

            var prices = new MarketPrices();
            
            var rPairsDict = r.markets.ToDictionary(x => x.marketname.ToAssetPair(this), x => x);
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
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last_price)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.ask, currentTicker.bid, currentTicker.low24h, currentTicker.high24h),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.volume24h)
                    });
                }
            }

            return prices;
        }*/
    }
}
