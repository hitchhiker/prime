﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;

namespace Prime.Finance.Services.Services.LiveCoin
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://www.livecoin.net/api/public
    public class LiveCoinProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string LiveCoinApiUrl = "https://api.livecoin.net/exchange/";

        private static readonly ObjectId IdHash = "prime:livecoin".GetObjectIdHashCode();

        //Only 60 API calls (any calls) are allowed over a period of 60 seconds from one IP address.
        //https://www.livecoin.net/api/common
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(60, 60);

        private RestApiClientProvider<ILiveCoinApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("LiveCoin");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '/';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public LiveCoinProvider()
        {
            ApiProvider = new RestApiClientProvider<ILiveCoinApi>(LiveCoinApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync("BTC/USD").ConfigureAwait(false);

            return r?.last > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r)
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
            
            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.min_ask, r.max_bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var prices = new MarketPrices();

            var rPairsDict = r.ToDictionary(x => x.symbol.ToAssetPair(this), x => x);
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
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.min_ask, currentTicker.max_bid, currentTicker.low, currentTicker.high),
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

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var r = await api.GetOrderBookAsync(pairCode, maxCount).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var asks = r.asks;
            var bids = r.bids;

            foreach (var i in bids.Select(GetBidAskData))
                orderBook.AddBid(i.Item1, i.Item2, true);

            foreach (var i in asks.Select(GetBidAskData))
                orderBook.AddAsk(i.Item1, i.Item2, true);

            return orderBook;
        }

        private Tuple<decimal, decimal> GetBidAskData(float[] data)
        {
            decimal price = (decimal)data[0];
            decimal amount = (decimal)data[1];

            return new Tuple<decimal, decimal>(price, amount);
        }
    }
}
