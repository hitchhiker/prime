using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;
using System.Linq;

namespace Prime.Plugins.Services.BxInTh
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://bx.in.th/info/api/
    public class BxInThProvider : /*IPublicPricingProvider,*/ IAssetPairsProvider
    {
        private const string BxInThApiUrl = "https://bx.in.th/api/";

        private static readonly ObjectId IdHash = "prime:bxinth".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBxInThApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("BxInTh");

        public bool Disabled => true;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BxInThProvider()
        {
            ApiProvider = new RestApiClientProvider<IBxInThApi>(BxInThApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetCurrenciesAsync().ConfigureAwait(false);

            return r?.Count > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetCurrenciesAsync().ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r)
            {
                var t = rCurrentTicker.Value;
                pairs.Add(new AssetPair(t.primary_currency, t.secondary_currency, this));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        //private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        //{
        //    Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        //};

        //public PricingFeatures PricingFeatures => StaticPricingFeatures;

        //public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        //{
        //    var api = ApiProvider.GetApi(context);
        //    var r = await api.GetTickersAsync().ConfigureAwait(false);

        //    if (r == null || r.Count == 0)
        //    {
        //        throw new ApiResponseException("No tickers returned", this);
        //    }

        //    var prices = new MarketPrices();

        //    var rPairsDict = r.Values.ToDictionary(x =>  new AssetPair(x.primary_currency, x.secondary_currency, this), x => x);
        //    var pairsQueryable = context.IsRequestAll ? rPairsDict.Keys.ToList() : context.Pairs;

        //    foreach (var pair in pairsQueryable)
        //    {
        //        rPairsDict.TryGetValue(pair, out var currentTicker);

        //        if (currentTicker == null)
        //        {
        //            prices.MissedPairs.Add(pair);
        //        }
        //        else
        //        {
        //            //prices.Add(new MarketPrice(Network, pair, 1 / currentTicker.last_price)
        //            //{
        //            //   TODO
        //            //});
        //        }
        //    }

        //    return prices;
        //}
    }
}
