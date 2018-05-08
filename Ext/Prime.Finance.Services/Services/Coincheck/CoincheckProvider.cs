﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;

namespace Prime.Finance.Services.Services.Coincheck
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://coincheck.com/documents/exchange/api#public
    public class CoincheckProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string CoincheckApiUrl = "https://coincheck.com/api/";

        private static readonly ObjectId IdHash = "prime:coincheck".GetObjectIdHashCode();
        private const string PairsCsv = "btcjpy,ethjpy,etcjpy,daojpy,lskjpy,fctjpy,xmrjpy,repjpy,xrpjpy,zecjpy,xemjpy,ltcjpy,dashjpy,bchjpy,ethbtc,etcbtc,lskbtc,fctbtc,xmrbtc,repbtc,xrpbtc,zecbtc,xembtc,ltcbtc,dashbtc,bchbtc";

        // No information in API document.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<ICoincheckApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Coincheck");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator { get; }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public CoincheckProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoincheckApi>(CoincheckApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("btc_jpy".ToAssetPair(this, '_'));
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
            var pairCode = context.Pair.ToTicker(this, '_').ToLower();
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r == null)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);

            if (context.Pair.Equals(new AssetPair("BTC", "JPY")) == false)
            {
                throw new ApiResponseException("Invalid asset pair - only btc_jpy is supported.");
            }

            var r = await api.GetOrderBookAsync().ConfigureAwait(false);
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
