using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Gemini
{
    public class GeminiProvider : IDescribesAssets
    {
        private const string GemeniApiVerstion = "v1";
        private const string GeminiApiUrl = "https://api.gemini.com/" + GemeniApiVerstion;
        private RestApiClientProvider<IGeminiApi> ApiProvider { get; }

        private static readonly ObjectId IdHash = "prime:gemini".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("Gemini");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        /// <summary>
        /// For public API entry points, we limit requests to 120 requests per minute, and recommend that you do not exceed 1 request per second.
        /// For private API entry points, we limit requests to 600 requests per minute, and recommend that you not exceed 5 requests per second.
        /// See https://docs.gemini.com/rest-api/#rate-limits.
        /// </summary>
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(60, 1, 300, 1);
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;

        public GeminiProvider()
        {
            ApiProvider = new RestApiClientProvider<IGeminiApi>(GeminiApiUrl, this, k => null);
        }
        public Task<bool> TestPublicApiAsync()
        {
            var t = new Task<bool>(() => true);
            t.Start();
            return t;
        }
        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetSymbols();

            var pairs = new AssetPairs();
            foreach (var rSymbol in r)
            {
                var assetPair = ParseAssetPair(rSymbol);

                pairs.Add(assetPair);
            }

            return pairs;
        }

        private AssetPair ParseAssetPair(string raw)
        {
            if(raw.Length != 6)
                throw new ApiResponseException($"Invalid format of currency pair: {raw}", this);

            var asset1 = raw.Substring(0, 3);
            var asset2 = raw.Substring(3);

            return new AssetPair(asset1, asset2);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = GetGeminiPair(context.Pair);
            var r = await api.GetTicker(pairCode);

            return new MarketPrice(context.Pair, r.last);
        }

        private string GetGeminiPair(AssetPair pair)
        {
            return pair.TickerSimple().ToLower();
        }

        public BuyResult Buy(BuyContext ctx)
        {
            throw new NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
