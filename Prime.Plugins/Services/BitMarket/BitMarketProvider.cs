using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.BitMarket
{
    // https://www.bitmarket.net/docs.php?file=api_public.html
    public class BitMarketProvider : IAssetPairsProvider, IPublicPricingProvider
    {
        private const string BitMarketApiUrl = "https://www.bitmarket.net/";

        private RestApiClientProvider<IBitMarketApi> ApiProvider { get; }

        private const string PairsCsv = "BTC_PLN,BTC_EUR,LTC_PLN,LTC_BTC,LiteMineX_BTC";

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(PairsCsv.ToCsv().Select(x=> x.ToAssetPairRaw())));

        private static readonly ObjectId IdHash = "prime:bitmarket".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("BitMarket");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => false;
        private static readonly IAssetCodeConverter AssetCodeConverter = new BitMarketCodeConverter();

        public BitMarketProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitMarketApi>(BitMarketApiUrl, this, k => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }

        public Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            return Task.Run(() => true);
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanSatistics = true, CanVolume = true },
            Bulk = new PricingBulkFeatures()
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this, "");

            try
            {
                var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);
                var price = new MarketPrice(Network, context.Pair, r.last)
                {
                    PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                    Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
                };

                return new MarketPricesResult(price);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    throw new NoAssetPairException(context.Pair, this);
                throw;
            }
        }
    }
}
