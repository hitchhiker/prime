using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Kucoin
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://kucoinapidocs.docs.apiary.io
    public class KucoinProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string KucoinApiVersion = "v1";
        private const string KucoinApiUrl = "https://api.kucoin.com/" + KucoinApiVersion;

        private static readonly ObjectId IdHash = "prime:kucoin".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IKucoinApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Kucoin");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '-';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public KucoinProvider()
        {
            ApiProvider = new RestApiClientProvider<IKucoinApi>(KucoinApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync("kcs-btc").ConfigureAwait(false);

            return r.success && r.data != null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r.success == false || r.data == null)
            {
                throw new ApiResponseException(r.msg, this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.data)
            {
                pairs.Add(rCurrentTicker.symbol.ToAssetPair(this));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
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

            if (r.success == false || r.data == null)
            {
                throw new ApiResponseException(r.msg, this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.data.lastDealPrice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.data.sell, r.data.buy, r.data.low, r.data.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.data.vol)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r.success == false || r.data == null)
            {
                throw new ApiResponseException(r.msg, this);
            }

            var prices = new MarketPrices();

            var rPairsDict = r.data.ToDictionary(x => x.symbol.ToAssetPair(this), x => x);
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
                    prices.Add(new MarketPrice(Network, pair, currentTicker.lastDealPrice)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.sell, currentTicker.buy, currentTicker.low, currentTicker.high),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.vol)
                    });
                }
            }

            return prices;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var r = await api.GetOrderBookAsync(pairCode, maxCount).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);
            
            var asks = r.data.SELL.Take(maxCount);
            var bids = r.data.BUY.Take(maxCount);

            foreach (var i in bids.Select(GetBidAskData))
                orderBook.AddBid(i.Item1, i.Item2, true);

            foreach (var i in asks.Select(GetBidAskData))
                orderBook.AddAsk(i.Item1, i.Item2, true);

            return orderBook;
        }

        private Tuple<decimal, decimal> GetBidAskData(decimal[] data)
        {
            decimal price = data[0];
            decimal volume = data[2];

            return new Tuple<decimal, decimal>(price, volume);
        }
    }
}
