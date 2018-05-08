﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;

namespace Prime.Finance.Services.Services.Luno
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    //https://www.luno.com/en/api
    public class LunoProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string LunoApiVersion = "1";
        private const string LunoApiUrl = "https://api.mybitx.com/api/" + LunoApiVersion;

        private static readonly ObjectId IdHash = "prime:luno".GetObjectIdHashCode();

        //Calls to the Market Data APIs are rate limited to 1 call per 10 seconds.
        //https://www.luno.com/en/api#rate-limiting
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(1, 10);

        private RestApiClientProvider<ILunoApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Luno");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public LunoProvider()
        {
            ApiProvider = new RestApiClientProvider<ILunoApi>(LunoApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r?.tickers?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r?.tickers == null || r.tickers.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.tickers)
            {
                pairs.Add(rCurrentTicker.pair.ToAssetPair(this, 3));
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
            
            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last_trade)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, null, null),
                Volume = new NetworkPairVolume(Network, context.Pair, r.rolling_24_hour_volume)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r?.tickers == null || r.tickers.Length == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var prices = new MarketPrices();

            var rPairsDict = r.tickers.ToDictionary(x => x.pair.ToAssetPair(this, 3), x => x);
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
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last_trade)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.ask, currentTicker.bid),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.rolling_24_hour_volume)
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

            foreach (var i in bids)
                orderBook.AddBid(i.price, i.volume, true);

            foreach (var i in asks)
                orderBook.AddAsk(i.price, i.volume, true);

            return orderBook;
        }
    }
}
