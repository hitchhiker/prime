using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Bithumb
{
    // https://www.bithumb.com/u1/US127
    public class BithumbProvider : IAssetPairsProvider, IPublicPricingProvider
    {
        private const string BithumbApiUrl = "https://api.bithumb.com/";
        private RestApiClientProvider<IBithumbApi> ApiProvider { get; }

        private static readonly ObjectId IdHash = "prime:bithumb".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("Bithumb");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var r = await GetAssetPairsAsync(context).ConfigureAwait(false);
            
            return r.Count > 0;
        }

        // 20 requests available per second.
        // See https://www.bithumb.com/u1/US127.
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(1200, 1);

        public IRateLimiter RateLimiter => Limiter;

        public BithumbProvider()
        {
            ApiProvider = new RestApiClientProvider<IBithumbApi>(BithumbApiUrl, this, k => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            // TODO: add "status" field checking.

            var pairs = new AssetPairs();
            var krwAsset = Asset.Krw;

            foreach (var rTicker in r.data)
            {
                var pair = new AssetPair(rTicker.Key.ToAsset(this), krwAsset);
                pairs.Add(pair);
            }

            return pairs;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, SupportsMultipleQuotes = false}
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetTickersAsync().ConfigureAwait(false);

            // TODO: add "status" field checking.

            var r = ParseTickerResponse(rRaw);

            var krwAsset = Asset.Krw;

            var prices = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var rTickers = r.Where(x => x.Key.ToAsset(this).Equals(pair.Asset1)).ToArray();
                if (!rTickers.Any() || !pair.Asset2.Equals(krwAsset))
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                var rTicker = rTickers[0];
                var vals = rTicker.Value;
                prices.MarketPrices.Add(new MarketPrice(Network,  pair, vals.sell_price)
                {
                    Volume = new NetworkPairVolume(Network, pair, vals.volume_1day),
                    PriceStatistics = new PriceStatistics(Network, pair.Asset2, null, null, vals.min_price, vals.max_price)
                });
            }

            return prices;
        }

        [Obsolete("READ NOTE")]
        public async Task<MarketPricesResult> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var currency = context.Pair.Asset1.ToRemoteCode(this);

            var r = await api.GetTickerAsync(currency).ConfigureAwait(false);

            // TODO: add "status" field checking.

            var krwAsset = Asset.Krw;

            if (!context.Pair.Asset2.Equals(krwAsset))
                throw new NoAssetPairException(context.Pair, this);

            var data = r.data;
            
            var latestPrice = new MarketPrice(Network, context.Pair, r.data.sell_price)
            {
                //TODO: HH: Are you sure 'sell_price' == Lowest Ask -> I don't think it does. Leave null if not
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, data.sell_price, data.buy_price, data.min_price, data.max_price),
                Volume = new NetworkPairVolume(Network, context.Pair, data.volume_1day)
            };

            return new MarketPricesResult(latestPrice);
        }

        private Dictionary<string, BithumbSchema.TickerResponse> ParseTickerResponse(BithumbSchema.TickersResponse raw)
        {
            var dict = new Dictionary<string, BithumbSchema.TickerResponse>();

            foreach (var rPair in raw.data)
            {
                try
                {
                    var parsedValue = JsonConvert.DeserializeObject<BithumbSchema.TickerResponse>(rPair.Value.ToString());

                    dict.Add(rPair.Key, parsedValue);
                }
                catch
                {
                    continue;
                }
            }

            return dict;
        }
    }
}