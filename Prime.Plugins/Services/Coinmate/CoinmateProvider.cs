using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Coinmate
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://coinmate.docs.apiary.io/
    public class CoinmateProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string CoinmateApiUrl = "https://coinmate.io/api/";

        private static readonly ObjectId IdHash = "prime:coinmate".GetObjectIdHashCode();

        //Please not make more than 100 request per minute or we will ban your IP address.
        //https://coinmate.docs.apiary.io/#reference/request-limits
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(100, 1);

        private RestApiClientProvider<ICoinmateApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Coinmate");
        private const string PairsCsv = "BTCEUR,BTCCZK";

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public CoinmateProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinmateApi>(CoinmateApiUrl, this, (k) => null);
        }

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync("BTC_EUR").ConfigureAwait(false);

            return r?.error == false && r.data != null;
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
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = false }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r == null || r.error || r.data == null)
            {
                throw new ApiResponseException("No tickers found", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.data.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.data.ask, r.data.bid, r.data.low, r.data.high)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var r = await api.GetOrderBookAsync(pairCode,false).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            if (r == null || r.error || r.data == null)
            {
                throw new ApiResponseException(r.errorMessage, this);
            }

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var response = r.data;

            var asks = response.asks.Take(maxCount);
            var bids = response.bids.Take(maxCount);

            foreach (var i in bids)
                orderBook.AddBid(i.price, i.amount, true);

            foreach (var i in asks)
                orderBook.AddAsk(i.price, i.amount, true);

            return orderBook;
        }
    }
}
