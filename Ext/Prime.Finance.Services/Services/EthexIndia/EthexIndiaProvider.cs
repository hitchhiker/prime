﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;

namespace Prime.Finance.Services.Services.EthexIndia
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://api.ethexindia.com/
    public class EthexIndiaProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string EthexIndiaApiUrl = "https://api.ethexindia.com";

        private static readonly ObjectId IdHash = "prime:ethexindia".GetObjectIdHashCode();
        private const string PairsCsv = "ethinr";

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IEthexIndiaApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("EthexIndia");

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

        public EthexIndiaProvider()
        {
            ApiProvider = new RestApiClientProvider<IEthexIndiaApi>(EthexIndiaApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r?.Length > 0;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);
            
            var prices = new MarketPrices();

            //Treats results as dictionary just incase the API add more asset pairs later on.
            var rPairsDict = r.ToDictionary(x => x.ticker.ToAssetPair(this, 3), x => x);
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
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last_traded_price)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.ask, currentTicker.bid, currentTicker.low, currentTicker.high),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.total_volume_24h)
                    });
                }
            }

            return prices;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);

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
            decimal amount = data[0];
            decimal price = data[1];

            return new Tuple<decimal, decimal>(price, amount);
        }
    }
}
