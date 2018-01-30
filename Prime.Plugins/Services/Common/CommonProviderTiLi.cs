using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Plugins.Services.Base;

namespace Prime.Plugins.Services.Common
{
    /// <summary>
    /// Common provider for Tidex and Liqui exchanges.
    /// </summary>
    public abstract partial class CommonProviderTiLi<TApi> : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider where TApi : class, ICommonApiTiLi
    {
        //From doc: All information is cached every 2 seconds, so there's no point in making more frequent requests.
        //https://tidex.com/public-api
        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(1, 2);

        public abstract Network Network { get; }

        public virtual bool Disabled => false;
        public virtual int Priority => 100;
        public virtual string AggregatorName => null;
        public string Title => Network.Name;
        public abstract ObjectId Id { get; }
        public virtual IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public virtual char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        protected abstract RestApiClientProvider<TApi> ApiProviderPublic { get; }
        protected abstract RestApiClientProvider<TApi> ApiProviderPrivate { get; }

        protected Dictionary<string, object> CreatePostBody()
        {
            return new Dictionary<string, object>();
        }

        public virtual async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var r = await GetBalancesAsync(context).ConfigureAwait(false);

            return r != null;
        }

        protected void CheckResponse<T>(CommonSchemaTiLi.BaseResponse<T> r)
        {
            if (r.success != 1)
                throw new ApiResponseException(r.error, this);
        }

        protected CommonProviderTiLi()
        {
            // ApiProviderPublic = InitApiProviderPublic();// new RestApiClientProvider<ITidexApi>(TidexApiUrlPublic);
            // ApiProviderPrivate = InitApiProviderPrivate();  // new RestApiClientProvider<ITidexApi>(TidexApiUrlPrivate, this, (k) => new TidexAuthenticator(k).GetRequestModifierAsync);
        }

        public virtual async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProviderPublic.GetApi(context);
            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            return r?.pairs?.Count > 0;
        }

        public virtual async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProviderPublic.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            if (r == null || r.pairs.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (string entry in r.pairs.Keys)
            {
                pairs.Add(entry.ToAssetPair(this));
            }

            return pairs;
        }

        public virtual IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public virtual PricingFeatures PricingFeatures { get; } = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true }
        };

        public virtual async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        protected virtual async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProviderPublic.GetApi(context);
            var pairsCsv = string.Join("-", context.Pairs.Select(x => x.ToTicker(this).ToLower()));
            var r = await api.GetTickerAsync(pairsCsv).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var ticker = r.FirstOrDefault().Value;

            if (ticker != null)
            {
                return new MarketPrices(new MarketPrice(Network, context.Pair, ticker.last)
                {
                    PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, ticker.sell, ticker.buy, ticker.low, ticker.high),
                    Volume = new NetworkPairVolume(Network, context.Pair, ticker.vol)
                });
            }
            else
            {
                throw new ApiResponseException("No data returned", this);
            }
        }

        protected virtual async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProviderPublic.GetApi(context);
            var pairsCsv = string.Join("-", context.Pairs.Select(x => x.ToTicker(this).ToLower()));
            var r = await api.GetTickerAsync(pairsCsv).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var prices = new MarketPrices();

            foreach (var pair in context.Pairs)
            {
                var currentTicker = r.FirstOrDefault(x => x.Key.ToAssetPair(this).Equals(pair)).Value;

                if (currentTicker == null)
                {
                    prices.MissedPairs.Add(pair);
                }
                else
                {
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.sell, currentTicker.buy, currentTicker.low, currentTicker.high),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.vol)
                    });
                }
            }

            return prices;
        }

        public virtual async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProviderPublic.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            r.TryGetValue(pairCode, out var response);

            if (response == null)
            {
                throw new ApiResponseException("No depth info found");
            }

            var asks = response.asks.Take(maxCount);
            var bids = response.bids.Take(maxCount);

            foreach (var i in bids.Select(GetBidAskData))
                orderBook.AddBid(i.Item1, i.Item2, true);

            foreach (var i in asks.Select(GetBidAskData))
                orderBook.AddAsk(i.Item1, i.Item2, true);

            return orderBook;
        }

        protected Tuple<decimal, decimal> GetBidAskData(decimal[] data)
        {
            decimal price = data[0];
            decimal amount = data[1];

            return new Tuple<decimal, decimal>(price, amount);
        }
    }
}
