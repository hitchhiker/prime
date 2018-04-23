﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Finance.Services.Services.Yobit
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://www.yobit.net/en/api/
    public partial class YobitProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider, INetworkProviderPrivate
    {
        private const string YobitApiVersion = "3";
        private const string YobitApiUrl = "https://yobit.net/api/" + YobitApiVersion;
        private const string YobitApiPrivateUrl = "https://yobit.net/tapi/";

        private static readonly ObjectId IdHash = "prime:yobit".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IYobitApi> ApiProvider { get; }
        private RestApiClientProvider<IYobitApi> ApiProviderPrivate { get; }

        public Network Network { get; } = Networks.I.Get("Yobit");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public YobitProvider()
        {
            Encoding.RegisterProvider(new YobitEncodingProvider());
            ApiProvider = new RestApiClientProvider<IYobitApi>(YobitApiUrl, this, (k) => null);
            ApiProviderPrivate = new RestApiClientProvider<IYobitApi>(YobitApiPrivateUrl, this, (k) => new YobitAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            return r?.pairs?.Count > 0;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp());

            var body = new Dictionary<string, object>
            {
                { "method","getInfo" },
                { "nonce", timeStamp }
            };

            var api = ApiProviderPrivate.GetApi(context);
            var r = await api.GetUserInfoAsync(body).ConfigureAwait(false);
         
            return r != null && r.success;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            if (r.pairs == null || r.pairs.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.pairs)
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
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true }
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

            if (r.Count == 0 || r.TryGetValue(pairCode, out var ticker) == false)
            {
                throw new ApiResponseException("No ticker returned", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, ticker.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, ticker.sell, ticker.buy, ticker.low, ticker.high),
                Volume = new NetworkPairVolume(Network, context.Pair, ticker.vol)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairsCsv = string.Join("-", context.Pairs.Select(x => x.ToTicker(this).ToLower()));
            var r = await api.GetTickerAsync(pairsCsv).ConfigureAwait(false);

            if (r.Count == 0)
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
            var pairCode = context.Pair.ToTicker(this).ToLower();

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            if (r.Count == 0 || r.TryGetValue(pairCode, out var response) == false)
            {
                throw new ApiResponseException("No order book returned", this);
            }

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var asks = response.asks.Take(maxCount);
            var bids = response.bids.Take(maxCount);

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
