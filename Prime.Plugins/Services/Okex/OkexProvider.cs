using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Okex
{
    // https://www.okex.com/rest_api.html
    public class OkexProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string OkexApiVersion = "v1";
        private const string OkexApiUrl = "https://www.okex.com/api/" + OkexApiVersion;

        private static readonly ObjectId IdHash = "prime:okex".GetObjectIdHashCode();
        private const string PairsCsv = "ltcbtc,ethbtc,etcbtc,bchbtc,btcusdt,ethusdt,ltcusdt,etcusdt,bchusdt,etceth,bt1btc,bt2btc,btgbtc,qtumbtc,hsrbtc,neobtc,gasbtc,qtumusdt,hsrusdt,neousdt,gasusdt";

        //Each IP can send maximum of 3000 https requests within 5 minutes. 
        //If the 3000 limit is exceeded, the system will automatically block the IP for one hour. After that hour, the IP will be automatically unfrozen.
        //https://www.okex.com/rest_faq.html
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(3000, 5);

        private RestApiClientProvider<IOkexApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Okex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public string CommonPairSeparator { get; }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public OkexProvider()
        {
            ApiProvider = new RestApiClientProvider<IOkexApi>(OkexApiUrl, this, (k) => null);
        }

        public Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            // TODO: implement public api test.

            return Task.Run(() => true);
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this,"_");
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.ticker.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ticker.sell, r.ticker.buy, r.ticker.low, r.ticker.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.ticker.vol)
            });
        }
    }
}
