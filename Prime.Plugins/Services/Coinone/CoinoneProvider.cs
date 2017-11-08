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
        public Network Network { get; } = Networks.I.Get("Coinone");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;

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

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            if (!context.Pair.Asset2.Equals(Asset.Krw))
                throw new ApiResponseException($"Specified currency pair {context.Pair} is not supported by provider", this);

            var r = await api.GetTicker(context.Pair.Asset1.ShortCode);

            CheckResponseErrors(r);

            return new MarketPrice(context.Pair, r.last);
        }

        public async Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return await GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetTickers();
            CheckResponseErrors(rRaw);

            var r = ParseTicker(rRaw);

            var prices = new MarketPricesResult();
            var krwAsset = Asset.Krw;

            foreach (var pair in context.Pairs)
            {
                var ticker = new CoinoneSchema.TickerEntryResponse();
                if (!r.TryGetValue(pair.Asset1.ShortCode.ToLower(), out ticker) || !pair.Asset2.Equals(krwAsset))
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.MarketPrices.Add(new MarketPrice(pair, ticker.last));
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

        public async Task<VolumeResult> GetVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);

            if (!context.Pair.Asset2.Equals(Asset.Krw))
                throw new ApiResponseException($"Specified currency pair {context.Pair} is not supported by provider", this);

            var r = await api.GetTicker(context.Pair.Asset1.ShortCode);

            CheckResponseErrors(r);

            return new VolumeResult()
            {
                Pair = context.Pair,
                Volume = r.volume,
                Period = VolumePeriod.Day
            };
        }
    }
}
