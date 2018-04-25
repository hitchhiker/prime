﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Finance.Services.Services.Braziliex
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://braziliex.com/exchange/api.php
    public partial class BraziliexProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider, INetworkProviderPrivate
    {
        private const string BraziliexApiVersion = "v1";
        private const string BraziliexApiUrl = "https://braziliex.com/api/" + BraziliexApiVersion;

        private static readonly ObjectId IdHash = "prime:braziliex".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBraziliexApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Braziliex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BraziliexProvider()
        {
            ApiProvider = new RestApiClientProvider<IBraziliexApi>(BraziliexApiUrl, this, (k) => new BraziliexAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r?.Count > 0;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreateBody();
            body.Add("command", "balance");

            var rRaw = await api.GetBalancesAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r != null;
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

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r)
            {
                pairs.Add(rCurrentTicker.Key.ToAssetPair(this));
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

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.lowestAsk, r.highestBid),
                Volume = new NetworkPairVolume(Network, context.Pair, r.baseVolume24, r.quoteVolume24)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

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
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.lowestAsk, currentTicker.highestBid),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.baseVolume24, currentTicker.quoteVolume24)
                    });
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

            var asks = r.asks.Take(maxCount);
            var bids = r.bids.Take(maxCount);

            foreach (var i in bids)
                orderBook.AddBid(i.price, i.amount, true);

            foreach (var i in asks)
                orderBook.AddAsk(i.price, i.amount, true);

            return orderBook;
        }
    }
}
