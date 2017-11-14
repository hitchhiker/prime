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
        IPublicPriceProvider
    {
        private const string QuadrigaCxApiVersion = "v2";
        private const string QuadrigaCxApiUrl = "https://api.quadrigacx.com/" + QuadrigaCxApiVersion;

        private static readonly ObjectId IdHash = "prime:quadrigacx".GetObjectIdHashCode();

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
        
        public bool CanGenerateDepositAddress => true; //To confirm
        public bool CanPeekDepositAddress => false; //To confirm
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public QuadrigaCxProvider()
        {
            ApiProvider = new RestApiClientProvider<IQuadrigaCxApi>(QuadrigaCxApiUrl, this, (k) => null);
        }

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.Asset1.ToRemoteCode(this) + "_" + context.Pair.Asset2.ToRemoteCode(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);
            
            return new MarketPrice(Network, context.Pair.Asset1, new Money(1 / r.last, context.Pair.Asset2));
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool DoesMultiplePairs => false; //To confirm

        public bool PricesAsAssetQuotes => false; //To confirm
    }
}
