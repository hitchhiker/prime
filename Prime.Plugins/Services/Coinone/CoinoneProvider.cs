using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Coinone
{
    public class CoinoneProvider : IExchangeProvider, IPublicPricesProvider
    {
        private const string CoinoneApiUrl = "https://api.coinone.co.kr";

        private static readonly ObjectId IdHash = "prime:coinone".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("Coinone");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(90, 1);
        public IRateLimiter RateLimiter => Limiter;

        private RestApiClientProvider<ICoinoneApi> ApiProvider { get; }

        public CoinoneProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinoneApi>(CoinoneApiUrl, this, k => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetTickers();

            CheckResponseErrors(rRaw);

            var r = ParseTicker(rRaw);

            var pairs = new AssetPairs();
            var krwAsset = Asset.Krw;

            foreach (var kvp in r)
            {
                var pair = kvp.Key.ToAsset(this).ToPair(krwAsset);
                pairs.Add(pair);
            }

            return pairs;
        }

        private Dictionary<string, CoinoneSchema.TickerEntryResponse> ParseTicker(CoinoneSchema.TickersResponse rRaw)
        {
            var dict = new Dictionary<string, CoinoneSchema.TickerEntryResponse>();
            var keywords = new string[]
            {
                "result",
                "errorCode",
                "timestamp"
            };

            foreach (var kvp in rRaw)
            {
                if (keywords.Contains(kvp.Key))
                    continue;

                var ticker = JsonConvert.DeserializeObject<CoinoneSchema.TickerEntryResponse>(kvp.Value.ToString());
                dict.Add(kvp.Key, ticker);
            }

            return dict;
        }

        private void CheckResponseErrors(Dictionary<string, object> dictResponse)
        {
            object result = String.Empty;
            object errorCode = String.Empty;

            if (!dictResponse.TryGetValue("result", out result))
                throw new FormatException("Dictionary does not contain `result` key");
            if (!dictResponse.TryGetValue("errorCode", out errorCode))
                throw new FormatException("Dictionary does not contain `errorCode` key");

            var baseResponse = new CoinoneSchema.BaseResponse()
            {
                result = result.ToString(),
                errorCode = errorCode.ToString()
            };

            CheckResponseErrors(baseResponse);
        }

        private void CheckResponseErrors(CoinoneSchema.BaseResponse baseResponse)
        {
            if (!baseResponse.result.Equals("success") && !baseResponse.errorCode.Equals("0"))
                throw new ApiResponseException($"Error {baseResponse.errorCode}", this);
        }

        public async Task<LatestPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var krwAsset = Asset.Krw;

            if (!context.Pair.Asset2.Equals(krwAsset))
                    throw new ApiResponseException("Exchange does not support quote currencies other than KRW", this);

            var r = await api.GetTicker(context.Pair.Asset1.ShortCode);

            CheckResponseErrors(r);

            var price = new LatestPrice()
            {
                UtcCreated = DateTime.UtcNow,
                Price = new Money(r.last, krwAsset),
                QuoteAsset = context.Pair.Asset1
            };

            return price;
        }

        public async Task<List<LatestPrice>> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return await GetPricesAsync(context);
        }

        public async Task<List<LatestPrice>> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetTickers();
            CheckResponseErrors(rRaw);

            var r = ParseTicker(rRaw);

            var prices = new List<LatestPrice>();
            var krwAsset = Asset.Krw;

            foreach (var pair in context.Pairs)
            {
                if (!pair.Asset2.Equals(krwAsset))
                    throw new ApiResponseException("Exchange does not support quote currencies other than KRW", this);

                var ticker = new CoinoneSchema.TickerEntryResponse();
                if(!r.TryGetValue(pair.Asset1.ShortCode.ToLower(), out ticker))
                    throw new ApiResponseException($"Exchange does not support {pair} currency pair", this);

                prices.Add(new LatestPrice()
                {
                    UtcCreated = DateTime.UtcNow,
                    Price = new Money(ticker.last, pair.Asset2),
                    QuoteAsset = pair.Asset1
                });
            }

            return prices;
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
