using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BitBay
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://bitbay.net/en/api-public#details
    public partial class BitBayProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider, INetworkProviderPrivate
    {
        private const string BitBayApiUrl = "https://bitbay.net/API";

        private static readonly ObjectId IdHash = "prime:bitbay".GetObjectIdHashCode();

        //Nothing mentioned in documentation.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBitBayApi> ApiProvider { get; }
        //List was not found anywhere - just put together based on crypto currencies supported by BitBay according to https://en.bitcoin.it/wiki/BitBay.
        private const string PairsCsv = "LTCPLN,LTCUSD,LTCEUR,BTCPLN,BTCUSD,BTCEUR,ETHPLN,ETHUSD,ETHEUR,LSKPLN,LSKUSD,LSKEUR,BCCPLN,BCCUSD,BCCEUR";

        public Network Network { get; } = Networks.I.Get("BitBay");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public char? CommonPairSeparator => null;

        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var timestamp = (long)DateTime.UtcNow.ToUnixTimeStamp();

            var body = new Dictionary<string, object>
            {
                { "method", "info" },
                { "moment", timestamp}
            };

            var api = ApiProvider.GetApi(context);
            var r = await api.GetUserInfoAsync(body).ConfigureAwait(false);

            return r != null && r.success && r.balances.Count > 0;
        }

        public BitBayProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitBayApi>(BitBayApiUrl, this, (k) => new BitBayAuthenticator(k).GetRequestModifierAsync);
        }

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync("BTCUSD").ConfigureAwait(false);

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
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.min, r.max),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var asks = r.asks.Take(maxCount);
            var bids = r.bids.Take(maxCount);

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
