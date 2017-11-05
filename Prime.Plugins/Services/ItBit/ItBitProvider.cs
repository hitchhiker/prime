using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.ItBit
{
    public class ItBitProvider : IExchangeProvider
    {
        private readonly string _pairs = "xbtusd,xbtsgd,xbteur";
        private readonly string ItBitApiUrl = "https://api.itbit.com/v1/";

        private static readonly ObjectId IdHash = "prime:itbit".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);

        public Network Network { get; } = new Network("ItBit");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        private RestApiClientProvider<IItBitApi> ApiProvider;

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;

        public ItBitProvider()
        {
            ApiProvider = new RestApiClientProvider<IItBitApi>(ItBitApiUrl, this, k => null);
        }

        private static readonly IAssetCodeConverter AssetCodeConverter = new ItBitCodeConverter();
        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = GetItBitTicker(context.Pair);

            var r = await api.GetTicker(pairCode);

            return new MarketPrice(context.Pair, r.lastPrice);
        }

        private string GetItBitTicker(AssetPair pair)
        {
            return $"{pair.Asset1.ToRemoteCode(this)}{pair.Asset2.ToRemoteCode(this)}";
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
