﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;

namespace Prime.Finance.Services.Services.NLexch
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://www.nlexch.com/documents/api_v2
    public partial class NLexchProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider, INetworkProviderPrivate
    {
        private const string NLexchApiVersion = "v2";
        private const string NLexchApiUrl = "https://www.nlexch.com:443//api/" + NLexchApiVersion;

        private static readonly ObjectId IdHash = "prime:nlexch".GetObjectIdHashCode();

        //Private API - 6000 requests/keypair/5minutes, you can contact NLexch if you need more.
        //Public API - unlimited
        //https://www.nlexch.com/documents/api_v2
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(6000,5);

        private RestApiClientProvider<INLexchApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("NLexch");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;
        
        public NLexchProvider()
        {
            ApiProvider = new RestApiClientProvider<INLexchApi>(NLexchApiUrl, this, (k) => new NLexchAuthenticator(k).GetRequestModifierAsync);
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
            var rRaw = await api.GetUserInfoAsync().ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.activated;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            var assetPairs = new AssetPairs();

            foreach (var rPrice in r.OrderBy(x => x.Key.Length))
            {
                var pair = AssetsUtilities.GetAssetPair(rPrice.Key, assetPairs);

                if (!pair.HasValue)
                    continue;

                assetPairs.Add(new AssetPair(pair.Value.AssetCode1, pair.Value.AssetCode2, this));
            }

            return assetPairs;
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

            if (r.ticker == null)
            {
                throw new ApiResponseException("No ticker returned", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.ticker.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ticker.sell, r.ticker.buy, r.ticker.low, r.ticker.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.ticker.vol)
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
            var knownPairs = new AssetPairs();

            if (context.IsRequestAll)
            {
                foreach (var rPrice in r.OrderBy(x => x.Key.Length))
                {
                    var tPair = AssetsUtilities.GetAssetPair(rPrice.Key, knownPairs);

                    if (!tPair.HasValue)
                        continue;

                    var pair = new AssetPair(tPair.Value.AssetCode1, tPair.Value.AssetCode2, this);

                    knownPairs.Add(pair);

                    prices.Add(new MarketPrice(Network, pair, rPrice.Value.ticker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, rPrice.Value.ticker.sell, rPrice.Value.ticker.buy, rPrice.Value.ticker.low, rPrice.Value.ticker.high),
                        Volume = new NetworkPairVolume(Network, pair, rPrice.Value.ticker.vol)
                    });
                }
            }
            else
            {
                foreach (var pair in context.Pairs)
                {
                    var lowerPairTicker = pair.ToTicker(this, "").ToLower();

                    var lpr = r.FirstOrDefault(x => x.Key.ToLower().Equals(lowerPairTicker));

                    if (lpr.Value == null)
                    {
                        prices.MissedPairs.Add(pair);
                        continue;
                    }

                    prices.Add(new MarketPrice(Network, pair, lpr.Value.ticker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, lpr.Value.ticker.sell, lpr.Value.ticker.buy, lpr.Value.ticker.low, lpr.Value.ticker.high),
                        Volume = new NetworkPairVolume(Network, pair, lpr.Value.ticker.vol)
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
                orderBook.AddBid(i.price, i.volume, true);

            foreach (var i in asks)
                orderBook.AddAsk(i.price, i.volume, true);

            return orderBook;
        }
    }
}
