using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Bisq
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://markets.bisq.network/api/
    //Revisit this later as per Frank's Slack message, since this is a decentralised exchange.
    public class BisqProvider : /*IPublicPricingProvider,*/ IAssetPairsProvider, IOrderBookProvider
    {
        private const string BisqApiUrl = "https://markets.bisq.network/api/";

        private static readonly ObjectId IdHash = "prime:bisq".GetObjectIdHashCode();

        //No rate limit found in documentation.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBisqApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Bisq");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public char? CommonPairSeparator => '_';

        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BisqProvider()
        {
            ApiProvider = new RestApiClientProvider<IBisqApi>(BisqApiUrl, this, (k) => null);
        }

        public Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            return Task.Run(() => true);
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAllTickers().ConfigureAwait(false);

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.Keys)
            {
                pairs.Add(rCurrentTicker.ToAssetPair(this));
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
            var pairCode = context.Pair.ToTicker(this).ToLower();
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            var ticker = r.FirstOrDefault();

            if (ticker?.last != null)
            {
                return new MarketPrices(new MarketPrice(Network, context.Pair, ticker.last.Value)
                {
                    PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, ticker.sell, ticker.buy,
                        ticker.low, ticker.high),
                    Volume = new NetworkPairVolume(Network, context.Pair, ticker.volume_left, ticker.volume_right)
                });
            }
            else
            {
                throw new ApiResponseException("'last' has been returned as null");
            }
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAllTickers().ConfigureAwait(false);

            var prices = new MarketPrices();

            var rPairsDict = r.ToDictionary(x => x.Key.ToAssetPair(this), x => x.Value);
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
                    if (currentTicker.last != null)
                    {
                        prices.Add(new MarketPrice(Network, pair, currentTicker.last.Value)
                        {
                            PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.sell,
                                currentTicker.buy, currentTicker.low, currentTicker.high),
                            Volume = new NetworkPairVolume(Network, pair, currentTicker.volume_left,
                                currentTicker.volume_right)
                        });
                    }
                    else
                    {
                        prices.MissedPairs.Add(pair);
                    }
                }
            }

            return prices;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            r.TryGetValue(pairCode, out var response);

            if (response == null)
            {
                throw new ApiResponseException("No depth info found");
            }

            var asks = response.sells.Take(maxCount);
            var bids = response.buys.Take(maxCount);

            foreach (var i in bids)
                orderBook.AddBid(i, 0, true);

            foreach (var i in asks)
                orderBook.AddAsk(i, 0, true);

            return orderBook;
        }
    }
}
