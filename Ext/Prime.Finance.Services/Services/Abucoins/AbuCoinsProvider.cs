﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Services.Services.Abucoins
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://docs.abucoins.com
    public class AbucoinsProvider : IPublicPricingProvider, IAssetPairsProvider, IPublicVolumeProvider, IOrderBookProvider
    {
        public Version Version { get; } = new Version(1, 0, 0);
        private const string AbucoinsApiUrl = "https://api.abucoins.com/";

        private static readonly ObjectId IdHash = "prime:abucoins".GetObjectIdHashCode();

        //300 requests per 1 minute per IP and Account. When a rate limit is exceeded, a status of 429 will be returned.
        //https://docs.abucoins.com/#rate-limits
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(300, 1);

        private RestApiClientProvider<IAbucoinsApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Abucoins");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '-';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public AbucoinsProvider()
        {
            ApiProvider = new RestApiClientProvider<IAbucoinsApi>(AbucoinsApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAssetsAsync().ConfigureAwait(false);

            return r?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetsAsync().ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r)
            {
                pairs.Add(rCurrentTicker.id.ToAssetPair(this));
            }

            return pairs;
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
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.price)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetVolumesAsync().ConfigureAwait(false);

            var rPairsDict = r.ToDictionary(x => x.product_id.ToAssetPair(this), x => x);

            var pairsQueryable = context.IsRequestAll ? rPairsDict.Keys.ToList() : context.Pairs;

            var volumes = new List<NetworkPairVolume>();
            var missingPairs = new List<AssetPair>();

            foreach (var pair in pairsQueryable)
            {
                if (!rPairsDict.TryGetValue(pair, out var ticker))
                {
                    missingPairs.Add(pair);
                    continue;
                }

                volumes.Add(new NetworkPairVolume(Network, pair, ticker.volume, null));
            }

            return new PublicVolumeResponse(Network, volumes, missingPairs);
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var r = await api.GetOrderBookAsync(pairCode, 0).ConfigureAwait(false);

            if(!r.bids.Any() && !r.asks.Any())
                throw new ApiResponseException("No order book data returned", this);

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

        private static readonly VolumeFeatures StaticVolumeFeatures = new VolumeFeatures()
        {
            Bulk = new VolumeBulkFeatures() { CanReturnAll = true, CanVolumeBase = true, CanVolumeQuote = true }
        };

        public VolumeFeatures VolumeFeatures => StaticVolumeFeatures;

    }
}
