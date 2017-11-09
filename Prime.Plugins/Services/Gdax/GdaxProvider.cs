using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Gdax
{
    public class GdaxProvider : IDescribesAssets
    {
        private const string GdaxApiUrl = "https://api.gdax.com";

        private static readonly ObjectId IdHash = "prime:gdax".GetObjectIdHashCode();

        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("GDAX");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        /// <summary>
        /// We throttle public endpoints by IP: 3 requests per second, up to 6 requests per second in bursts.
        /// We throttle private endpoints by user ID: 5 requests per second, up to 10 requests per second in bursts.
        /// See https://docs.gdax.com/#rate-limits.
        /// </summary>
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(180, 1, 300, 1);

        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;

        private RestApiClientProvider<IGdaxApi> ApiProvider { get; }

        public GdaxProvider()
        {
            ApiProvider = new RestApiClientProvider<IGdaxApi>(GdaxApiUrl, this, k => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public Task<bool> TestPublicApiAsync()
        {
            var t = new Task<bool>(() => true);
            t.Start();
            return t;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetProducts();

            var pairs = new AssetPairs();

            foreach (var rProduct in r)
            {
                pairs.Add(new AssetPair(rProduct.base_currency, rProduct.quote_currency));
            }

            return pairs;
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerDash();
            var r = await api.GetProductTicker(pairCode);

            return new MarketPrice(context.Pair, r.price);
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

            var pairCode = context.Pair.TickerDash();
            var r = await api.GetProductTicker(pairCode);

            return new VolumeResult()
            {
                Pair = context.Pair,
                Volume = r.volume,
                Period = VolumePeriod.Day
            };
        }
    }
}
