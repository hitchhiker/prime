using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Coinfloor
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://github.com/coinfloor/API/blob/master/BIST.md
    public class CoinfloorProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string CoinfloorApiUrl = "https://webapi.coinfloor.co.uk:8090/bist/";

        private static readonly ObjectId IdHash = "prime:coinfloor".GetObjectIdHashCode();
        private const string PairsCsv = "BTCeur,BTCgbp,BTCusd,BTCpln";

        // Information requests: 10 per 10 seconds per session
        //https://github.com/coinfloor/API/blob/master/BIST.md
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(10,10);
        
        private RestApiClientProvider<ICoinfloorApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Coinfloor");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        private static readonly IAssetCodeConverter AssetCodeConverter = new CoinfloorCodeConverter();
        public char? CommonPairSeparator => '\0';

        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public CoinfloorProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinfloorApi>(CoinfloorApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("BTC_GBP".ToAssetPair(this,'_'));
            var r = await GetPricingAsync(ctx).ConfigureAwait(false);

            return r != null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this,'/');

            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);
            
            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, '/');

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

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }
    }
}
