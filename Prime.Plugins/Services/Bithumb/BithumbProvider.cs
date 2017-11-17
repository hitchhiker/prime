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
    public class BithumbProvider : IPublicPricesProvider, IAssetPairsProvider, IPublicPriceProvider, IPublicPriceStatistics
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

            var pairs = new AssetPairs();
            var krwAsset = Asset.Krw;

            foreach (var rTicker in r.data)
            {
                var pair = new AssetPair(rTicker.Key.ToAsset(this), krwAsset);
                pairs.Add(pair);
            }

            return pairs;
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var currency = context.Pair.Asset1.ToRemoteCode(this);

            var r = await api.GetTickerAsync(currency).ConfigureAwait(false);

            var krwAsset = Asset.Krw;

            if (!context.Pair.Asset2.Equals(krwAsset))
                throw new NoAssetPairException(context.Pair, this);

            var data = r.data;

            var latestPrice = new MarketPrice(Network, context.Pair, r.data.sell_price)
            {
                PriceStatistics = new PriceStatistics(context.QuoteAsset, data.volume_1day, null, data.sell_price, data.buy_price, data.min_price, data.max_price)
            };

            return latestPrice;
        }

        public Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetTickersAsync().ConfigureAwait(false);
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

                var rTiker = rTickers[0];
                prices.MarketPrices.Add(new MarketPrice(Network,  pair, rTiker.Value.sell_price));
            }

            return prices;
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