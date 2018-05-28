﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Services.Services.MixCoins
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://mixcoins.com/help/api
    public class MixCoinsProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        public Version Version { get; } = new Version(1, 0, 0);
        private const string MixcoinsApiVersion = "v1";
        private const string MixcoinsApiUrl = "https://mixcoins.com/api/" + MixcoinsApiVersion;

        private static readonly ObjectId IdHash = "prime:mixcoins".GetObjectIdHashCode();
        private const string PairsCsv = "btcusd,ethbtc,bccbtc,lskbtc,bccusd,ethusd";

        // No information in API document.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IMixCoinsApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Mixcoins");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public MixCoinsProvider()
        {
            ApiProvider = new RestApiClientProvider<IMixCoinsApi>(MixcoinsApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("btc_usd".ToAssetPair(this));
            var r = await GetPricingAsync(ctx).ConfigureAwait(false);

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
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r == null || r.message.Equals("ok", StringComparison.InvariantCultureIgnoreCase) == false || r.result == null)
            {
                throw new ApiResponseException("No ticker found");
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.result.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.result.sell, r.result.buy, r.result.low, r.result.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.result.vol)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            if (r.message.Equals("ok", StringComparison.InvariantCultureIgnoreCase) == false || r.result == null)
            {
                throw new ApiResponseException(r.message);
            }

            var response = r.result;

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
