using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Cex
{
    public class CexProvider : IExchangeProvider, IPublicPricesProvider
    {
        private const string CexApiUrl = "https://cex.io/api";

        private static readonly ObjectId IdHash = "prime:cex".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("CEX.IO");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;

        private RestApiClientProvider<ICexApi> ApiProvider { get; }

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public CexProvider()
        {
            ApiProvider = new RestApiClientProvider<ICexApi>(CexApiUrl, this, k => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickers();

            var pairs = new AssetPairs();

            foreach (var rTicker in r.data)
            {
                pairs.Add(rTicker.pair.ToAssetPair(this, ':'));
            }

            return pairs;
        }

        public async Task<LatestPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = GetCexTicher(context.Pair);

            var r = await api.GetLastPrice(pairCode);

            return new LatestPrice(context.Pair, r.lprice);
        }

        private string GetCexTicher(AssetPair pair)
        {
            return $"{pair.Asset1.ToRemoteCode(this)}/{pair.Asset2.ToRemoteCode(this)}";
        }

        public async Task<List<LatestPrice>> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return await GetPricesAsync(context);
        }

        public async Task<List<LatestPrice>> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetLastPrices();
            CheckResponseError(r);

            var prices = new List<LatestPrice>();

            foreach (var pair in context.Pairs)
            {
                var rPair = r.data.FirstOrDefault(x =>
                    x.symbol1.ToAsset(this).Equals(pair.Asset1) && 
                    x.symbol2.ToAsset(this).Equals(pair.Asset2));

                if(rPair == null)
                    throw new ApiResponseException($"{pair} pair is not supported by this API", this);

                prices.Add(new LatestPrice(pair, rPair.lprice));
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

        private void CheckResponseError(CexSchema.BaseResponse response)
        {
            if(!response.ok.Equals("ok")) 
                throw new ApiResponseException($"Error occurred in provider: {response.ok}", this);
        }
    }
}
