﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;

namespace Prime.Finance.Services.Services.Exmo
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://exmo.com/en/api#/public_api
    public partial class ExmoProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider, INetworkProviderPrivate
    {
        private const string ExmoApiVersion = "v1";
        private const string ExmoApiUrl = "https://api.exmo.com/" + ExmoApiVersion;

        private static readonly ObjectId IdHash = "prime:exmo".GetObjectIdHashCode();

        //The number of API requests is limited to 180 per/minute from one IP address or by a single user.
        //https://exmo.com/en/api#/public_api
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(180, 1);

        private RestApiClientProvider<IExmoApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Exmo");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public ExmoProvider()
        {
            ApiProvider = new RestApiClientProvider<IExmoApi>(ExmoApiUrl, this, (k) => new ExmoAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetCurrencyAsync().ConfigureAwait(false);

            return r?.Length > 0;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetUserInfoAsync().ConfigureAwait(false);

            //CheckResponseErrors(rRaw);

            //var r = rRaw.GetContent();

            return r != null/* && r.success*/;
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

            foreach (var entry in r)
            {
                pairs.Add(entry.Key.ToAssetPair(this));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
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
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last_trade)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.sell_price, currentTicker.buy_price, currentTicker.low, currentTicker.high),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.vol)
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

            r.TryGetValue(pairCode, out var response);

            if (response == null)
            {
                throw new ApiResponseException("No depth info found");
            }

            var asks = response.ask.Take(maxCount);
            var bids = response.bid.Take(maxCount);

            foreach (var i in bids.Select(GetBidAskData))
                orderBook.AddBid(i.Item1, i.Item2, true);

            foreach (var i in asks.Select(GetBidAskData))
                orderBook.AddAsk(i.Item1, i.Item2, true);

            return orderBook;
        }

        private Tuple<decimal, decimal> GetBidAskData(decimal[] data)
        {
            decimal price = data[0];
            decimal amount = data[2];

            return new Tuple<decimal, decimal>(price, amount);
        }
    }
}
