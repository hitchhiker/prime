using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.CoinCorner
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://api.coincorner.com/
    public class CoinCornerProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string CoinCornerApiUrl = "https://api.coincorner.com/api";

        private static readonly ObjectId IdHash = "prime:coincorner".GetObjectIdHashCode();
        private const string PairsCsv = "btceur,btcgbp";

        // We do please ask you if using CoinCorners API not to make more than 60 requests a minute or we will be forced to ban your IP address. 
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(60, 1);

        private RestApiClientProvider<ICoinCornerApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("CoinCorner");

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

        public CoinCornerProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinCornerApi>(CoinCornerApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("btceur".ToAssetPair(this, 3));
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
            var r = await api.GetTickerAsync(context.Pair.Asset1.ShortCode, context.Pair.Asset2.ShortCode).ConfigureAwait(false);

            if (r == null)
            {
                throw new ApiResponseException("No ticker returned");
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.LastPrice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, null, r.BidHigh, r.LastLow, r.LastHigh),
                Volume = new NetworkPairVolume(Network, context.Pair, r.Volume)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetOrderBookAsync(context.Pair.Asset2.ShortCode.ToLower()).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var asks = r.sells.Take(maxCount);
            var bids = r.buys.Take(maxCount);

            foreach (var i in bids)
                orderBook.AddBid(i.Price, i.Amount, true);

            foreach (var i in asks)
                orderBook.AddAsk(i.Price, i.Amount, true);

            return orderBook;
        }
    }
}
