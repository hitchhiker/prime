using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Plugins.Services.Bittrex;
using Prime.Plugins.Services.QuadrigaCx;
using Prime.Utility;

namespace Prime.Plugins.Services.QuadrigaCX
{
    public class QuadrigaCxProvider :
        IPublicPriceProvider, IAssetPairsProvider
    {
        private const string QuadrigaCxApiVersion = "v2";
        private const string QuadrigaCxApiUrl = "https://api.quadrigacx.com/" + QuadrigaCxApiVersion;

        private static readonly ObjectId IdHash = "prime:quadrigacx".GetObjectIdHashCode();
        private const string PairsCsv = "btccad,btcusd,ethbtc,ethcad";

        // No information in API document.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IQuadrigaCxApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("QuadrigaCX");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;

        public bool IsDirect => true;
        
        public bool CanGenerateDepositAddress => true; // TODO: confirm
        public bool CanPeekDepositAddress => false; // TODO: confirm
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public QuadrigaCxProvider()
        {
            ApiProvider = new RestApiClientProvider<IQuadrigaCxApi>(QuadrigaCxApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("btc_cad".ToAssetPairRaw());
            var r = await GetPriceAsync(ctx).ConfigureAwait(false);

            return r != null;
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, "_");
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

        public bool DoesMultiplePairs => false; // TODO: confirm

        public bool PricesAsAssetQuotes => false; // TODO: confirm
    }
}
