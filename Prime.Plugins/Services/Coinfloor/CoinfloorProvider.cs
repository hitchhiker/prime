using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Coinfloor
{
    public class CoinfloorProvider :
        IPublicPriceProvider, IAssetPairsProvider
    {
        private const string CoinfloorApiUrl = "https://webapi.coinfloor.co.uk:8090/bist/";

        private static readonly ObjectId IdHash = "prime:coinfloor".GetObjectIdHashCode();
        private const string PairsCsv = "xbteur,xbtgbp,xbtusd,xbtpln";

        // Information requests: 10 per 10 seconds per session
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(60,1);

        private RestApiClientProvider<ICoinfloorApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Coinfloor");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;

        public bool IsDirect => true;

        public bool CanGenerateDepositAddress => true; //To confirm
        public bool CanPeekDepositAddress => false; //To confirm
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public CoinfloorProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinfloorApi>(CoinfloorApiUrl, this, (k) => null);
        }

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.TickerSlash(this);

            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPrice(Network, context.Pair, r.last);
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool DoesMultiplePairs => false; //To confirm

        public bool PricesAsAssetQuotes => false; //To confirm
    }
}
