using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;

namespace Prime.Finance.Services.Services.Dsx
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://dsx.docs.apiary.io/#
    public partial class DsxProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider, INetworkProviderPrivate
    {
        private const string ApiVersion = "v2";
        private const string DsxApiPublicUrl = "https://dsx.uk/mapi/";
        private const string DsxApiPrivateUrl = "https://dsx.uk/tapi/" + ApiVersion + "/";

        private static readonly ObjectId IdHash = "prime:dsx".GetObjectIdHashCode();

        //If you make more than 60 requests of Trading API methods (/tapi/) per minute using the same API Keys you will receive an error on each extra request during the same minute. Each new minute the counter becomes 0.
        //https://dsx.docs.apiary.io/#introduction/requests-limit
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(60, 1);

        private RestApiClientProvider<IDsxApi> ApiProviderPublic { get; }
        private RestApiClientProvider<IDsxApi> ApiProviderPrivate { get; }

        public Network Network { get; } = Networks.I.Get("Dsx");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public DsxProvider()
        {
            ApiProviderPublic = new RestApiClientProvider<IDsxApi>(DsxApiPublicUrl, this, (k) => null);
            ApiProviderPrivate = new RestApiClientProvider<IDsxApi>(DsxApiPrivateUrl, this, (k) => new DsxAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProviderPublic.GetApi(context);
            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            return r.pairs?.Count > 0;
        }

        private Dictionary<string, object> CreateBody()
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var body = new Dictionary<string, object>()
            {
                { "nonce", nonce }
            };

            return body;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);
            var body = CreateBody();
            var rRaw = await api.GetBalancesAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r != null && r.success == 1;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProviderPublic.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            if (r?.pairs == null || r.pairs.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.pairs)
            {
                pairs.Add(rCurrentTicker.Key.ToAssetPair(this,3));
            }

            return pairs;
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
            var api = ApiProviderPublic.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No ticker returned", this);
            }

            r.TryGetValue(pairCode, out var currentTicker);

            if (currentTicker == null)
            {
                throw new ApiResponseException("No ticker returned", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, currentTicker.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, currentTicker.sell, currentTicker.buy, currentTicker.low, currentTicker.high),
                Volume = new NetworkPairVolume(Network, context.Pair, currentTicker.vol)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProviderPublic.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            r.TryGetValue(pairCode, out var currentOrderBook);

            if (currentOrderBook == null)
            {
                throw new ApiResponseException("No order book returned", this);
            }

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var asks = currentOrderBook.asks.Take(maxCount);
            var bids = currentOrderBook.bids.Take(maxCount);

            foreach (var i in bids.Select(GetBidAskData))
                orderBook.AddBid(i.Item1, i.Item2, true);

            foreach (var i in asks.Select(GetBidAskData))
                orderBook.AddAsk(i.Item1, i.Item2, true);

            return orderBook;
        }

        private Tuple<decimal, decimal> GetBidAskData(decimal[] data)
        {
            decimal price = data[0];
            decimal amount = data[1];

            return new Tuple<decimal, decimal>(price, amount);
        }
    }
}
