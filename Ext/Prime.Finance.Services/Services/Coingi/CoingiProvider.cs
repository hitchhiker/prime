using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Services.Services.Coingi
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://coingi.docs.apiary.io/#reference
    public partial class CoingiProvider : IOrderBookProvider, INetworkProviderPrivate
    {
        public Version Version { get; } = new Version(1, 0, 0);
        private const string CoingiApiUrl = "https://api.coingi.com/";

        private static readonly ObjectId IdHash = "prime:Coingi".GetObjectIdHashCode();

        //If the user makes more than 300 requests per 1 minute the system will prevent them from making further calls.
        //https://coingi.docs.apiary.io/#introduction/request-limits
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(300, 1);

        private RestApiClientProvider<ICoingiApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Coingi");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public char? CommonPairSeparator => '-';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public CoingiProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoingiApi>(CoingiApiUrl, this, (k) => new CoingiAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTransactionListAsync("ltc-btc", 50).ConfigureAwait(false);

            return r != null && r.Length > 0;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var body = new Dictionary<string, object>
            {
                { "currencies","ltc,btc" }
            };

            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetBalancesAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r != null && r.Length > 0;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();

            var maxCount = Math.Min(50, context.MaxRecordsCount);

            var r = await api.GetOrderBookAsync(pairCode, maxCount, maxCount, 1).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var asks = r.asks;
            var bids = r.bids;

            foreach (var i in bids)
                orderBook.AddBid(i.price, i.counterAmount);

            foreach (var i in asks)
                orderBook.AddAsk(i.price, i.counterAmount);

            return orderBook;
        }
    }
}
