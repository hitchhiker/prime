﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Services.Services.BlinkTrade
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://blinktrade.com/docs/
    public partial class BlinkTradeProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider, INetworkProviderPrivate
    {
        public Version Version { get; } = new Version(1, 0, 0);
        private const string BlinkTradeApiVersion = "v1";
        private const string BlinkPublicApiUrl = "https://api.blinktrade.com/api/" + BlinkTradeApiVersion;
        private const string BlinkTradePrivateApiUrl = "https://api_testnet.blinktrade.com/tapi/" + BlinkTradeApiVersion + "/message";

        private static readonly ObjectId IdHash = "prime:blinktrade".GetObjectIdHashCode();
        private const string PairsCsv = "btcvef,btcvnd,btcbrl,btcpkr,btcclp";

        // No information in API document for REST API (there are rate limits for web sockets API though).
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBlinkTradeApi> ApiPublicProvider { get; }
        private RestApiClientProvider<IBlinkTradeApi> ApiPrivateProvider { get; }

        public Network Network { get; } = Networks.I.Get("BlinkTrade");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public BlinkTradeProvider()
        {
            ApiPublicProvider = new RestApiClientProvider<IBlinkTradeApi>(BlinkPublicApiUrl, this, (k) => null);
            ApiPrivateProvider = new RestApiClientProvider<IBlinkTradeApi>(BlinkTradePrivateApiUrl, this, (k) => new BlinkTradeAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("btcvef".ToAssetPair(this, 3));
            var r = await GetPricingAsync(ctx).ConfigureAwait(false);

            return r != null;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiPrivateProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "MsgType", "U2" },
                { "BalanceReqID", 1}
            };

            var rRaw = await api.GetBalanceAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r != null;
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiPublicProvider.GetApi(context);
            var r = await api.GetTickerAsync(context.Pair.Asset2.ShortCode, context.Pair.Asset1.ShortCode).ConfigureAwait(false);

            if (r == null || r.Count == 0)
            {
                throw new ApiResponseException("No ticker returned");
            }

            r.TryGetValue("high", out var high);
            r.TryGetValue("vol", out var vol);
            r.TryGetValue("buy", out var buy);
            r.TryGetValue("last", out var last);
            r.TryGetValue("low", out var low);
            r.TryGetValue("pair", out var pair);
            r.TryGetValue("sell", out var sell);
            r.TryGetValue("vol_" + context.Pair.Asset2.ShortCode.ToLower(), out var volFiat);

            return new MarketPrices(new MarketPrice(Network, context.Pair, Convert.ToDecimal(last))
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, Convert.ToDecimal(sell), Convert.ToDecimal(buy), Convert.ToDecimal(low), Convert.ToDecimal(high)),
                Volume = new NetworkPairVolume(Network, context.Pair, Convert.ToDecimal(vol), Convert.ToDecimal(volFiat))
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiPublicProvider.GetApi(context);

            var r = await api.GetOrderBookAsync(context.Pair.Asset2.ShortCode, context.Pair.Asset1.ShortCode).ConfigureAwait(false);
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
