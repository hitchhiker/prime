using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Bitso
{
    public class BitsoProvider :
        IPublicPriceProvider, IAssetPairsProvider
    {
        private const string BitsoApiVersion = "v3";
        private const string BitsoApiUrl = "https://api.bitso.com/" + BitsoApiVersion + "/";

        private static readonly ObjectId IdHash = "prime:bitso".GetObjectIdHashCode();

        // Rate limits are are based on one minute windows. 
        // If you do more than 300 requests in five minutes, you get locked out for one minute. 
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(300, 5);

        private RestApiClientProvider<IBitsoApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Bitso");

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

        public BitsoProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitsoApi>(BitsoApiUrl, this, (k) => null);
        }

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.TickerUnderslash(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPrice(Network, context.Pair.Asset1, new Money(1 / r.last, context.Pair.Asset2));
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            var pairs = new AssetPairs();

            foreach (var rCurrentPayloadResponse in r.payload)
            {
                pairs.Add(rCurrentPayloadResponse.book.ToAssetPairRaw());
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool DoesMultiplePairs => false; //To confirm

        public bool PricesAsAssetQuotes => false; //To confirm
    }
}
