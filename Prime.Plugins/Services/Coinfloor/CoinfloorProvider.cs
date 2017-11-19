using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Api.Request.RateLimits;
using Prime.Utility;

namespace Prime.Plugins.Services.Coinfloor
{
    // https://github.com/coinfloor/API/blob/master/BIST.md
    public class CoinfloorProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string CoinfloorApiUrl = "https://webapi.coinfloor.co.uk:8090/bist/";

        private static readonly ObjectId IdHash = "prime:coinfloor".GetObjectIdHashCode();
        private const string PairsCsv = "xbteur,xbtgbp,xbtusd,xbtpln";

        // Information requests: 10 per 10 seconds per session
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(10,10);
        
        private RestApiClientProvider<ICoinfloorApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Coinfloor");

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

        public CoinfloorProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinfloorApi>(CoinfloorApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("XBT_GBP".ToAssetPairRaw());
            var r = await GetPricingAsync(ctx).ConfigureAwait(false);

            return r != null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, "/");

            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
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
