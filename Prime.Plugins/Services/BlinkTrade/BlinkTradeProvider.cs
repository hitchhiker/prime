using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BlinkTrade
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://blinktrade.com/docs/
    public class BlinkTradeProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string BlinkTradeApiVersion = "v1";
        private const string BlinkTradeApiUrl = "https://api.blinktrade.com/api/" + BlinkTradeApiVersion;

        private static readonly ObjectId IdHash = "prime:blinktrade".GetObjectIdHashCode();
        private const string PairsCsv = "btcvef,btcvnd,btcbrl,btcpkr,btcclp";

        // No information in API document for REST API (there are rate limits for web sockets API though).
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBlinkTradeApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("BlinkTrade");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public BlinkTradeProvider()
        {
            ApiProvider = new RestApiClientProvider<IBlinkTradeApi>(BlinkTradeApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("btcvef".ToAssetPair(this, 3));
            var r = await GetPricingAsync(ctx).ConfigureAwait(false);

            return r != null;
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

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync(context.Pair.Asset2.ShortCode, context.Pair.Asset1.ShortCode).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No ticker returned");
            }

            r.TryGetValue("high", out var high);
            r.TryGetValue("vol", out var vol);
            r.TryGetValue("buy", out var buy);
            r.TryGetValue("last", out var last);
            r.TryGetValue("low", out var low);
            r.TryGetValue("pair", out var pair);
            r.TryGetValue("sell", out var sell);
            r.TryGetValue("vol_" + context.Pair.Asset2.ShortCode.ToLower(), out var volFiat);

            return new MarketPrices(new MarketPrice(Network, context.Pair, Convert.ToDecimal(last))
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, Convert.ToDecimal(sell), Convert.ToDecimal(buy), Convert.ToDecimal(low), Convert.ToDecimal(high)),
                Volume = new NetworkPairVolume(Network, context.Pair, Convert.ToDecimal(vol), Convert.ToDecimal(volFiat))
            });
        }
    }
}
