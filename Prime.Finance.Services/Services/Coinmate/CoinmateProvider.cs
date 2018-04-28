using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;

namespace Prime.Finance.Services.Services.Coinmate
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://coinmate.docs.apiary.io/
    public partial class CoinmateProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider, INetworkProviderPrivate
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

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard3;

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalanceAsync().ConfigureAwait(false);
           
            return r != null && r.error == false;
        }

        public CoinmateProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinmateApi>(CoinmateApiUrl, this, (k) => new CoinmateAuthenticator(k).GetRequestModifierAsync);
        }

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetTickerAsync("BTC_EUR").ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();
            
            return r.data != null;
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public IAssetCodeConverter GetAssetCodeConverter() => null;

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);
            var rRaw = await api.GetTickerAsync(pairCode).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();
            
            var data = r.data;

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.data.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, data.ask, data.bid, data.low, data.high),
                Volume = new NetworkPairVolume(Network, context.Pair, data.amount)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var rRaw = await api.GetOrderBookAsync(pairCode,false).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();
            
            var orderBook = new OrderBook(Network, context.Pair);

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
