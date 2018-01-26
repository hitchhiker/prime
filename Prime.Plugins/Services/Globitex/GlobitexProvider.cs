using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Globitex
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://globitex.com/api/
    public class GlobitexProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string GlobitexApiVersion = "1";
        private const string GlobitexApiUrl = "https://api.globitex.com/api/" + GlobitexApiVersion;

        private static readonly ObjectId IdHash = "prime:globitex".GetObjectIdHashCode();
        
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IGlobitexApi> ApiProvider { get; }

        private static readonly IAssetCodeConverter AssetCodeConverter = new GlobitexCodeConverter();

        public Network Network { get; } = Networks.I.Get("Globitex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public GlobitexProvider()
        {
            ApiProvider = new RestApiClientProvider<IGlobitexApi>(GlobitexApiUrl, this, (k) => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTimeAsync().ConfigureAwait(false);

            return r?.timestamp > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetSymbolsAsync().ConfigureAwait(false);

            if (r == null || r.symbols.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var currentSymbol in r.symbols)
            {
                pairs.Add(currentSymbol.symbol.ToAssetPair(this, 3));
            }

            return pairs;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        public async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r?.instruments == null || r.instruments.Length == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var prices = new MarketPrices();

            var rPairsDict = r.instruments.ToDictionary(x => x.symbol.ToAssetPair(this, 3), x => x);
            var pairsQueryable = context.IsRequestAll ? rPairsDict.Keys.ToList() : context.Pairs;

            foreach (var pair in pairsQueryable)
            {
                rPairsDict.TryGetValue(pair, out var currentTicker);

                if (currentTicker == null)
                {
                    prices.MissedPairs.Add(pair);
                }
                else
                {
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.ask, currentTicker.bid, currentTicker.low, currentTicker.high),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.volume)
                    });
                }
            }

            return prices;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

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
