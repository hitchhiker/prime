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
    public class BithumbProvider : IPublicPricesProvider
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

        public Task<bool> TestPublicApiAsync()
        {
            var t = new Task<bool>(() => true);
            t.Start();
            return t;
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
            var r = await api.GetTickers().ConfigureAwait(false);

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

            var r = await api.GetTicker(currency).ConfigureAwait(false);

            var krwAsset = Asset.Krw;

            if (!context.Pair.Asset2.Equals(krwAsset))
                throw new ApiResponseException($"Specified currency pair {context.Pair} is not supported by provider", this);

            var latestPrice = new MarketPrice(context.Pair, r.data.sell_price);

            return latestPrice;
        }

        public async Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return await GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetTickers().ConfigureAwait(false);
            var r = ParseTickerResponse(rRaw);

            var krwAsset = Asset.Krw;

            var prices = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var rTickers = r.Where(x => x.Key.ToAsset(this).Equals(pair.Asset1)).ToArray();
                if (rTickers.Length < 1 || !pair.Asset2.Equals(krwAsset))
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                var rTiker = rTickers[0];
                prices.MarketPrices.Add(new MarketPrice(pair, rTiker.Value.sell_price));
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

        public BuyResult Buy(BuyContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new System.NotImplementedException();
        }
    }
}