﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Finance.Services.Services.TuxExchange
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://tuxexchange.com/docs#
    public class TuxExchangeProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string TuxExchangeApiUrl = "https://tuxexchange.com/";

        private static readonly ObjectId IdHash = "prime:tuxexchange".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<ITuxExchangeApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("TuxExchange");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public TuxExchangeProvider()
        {
            ApiProvider = new RestApiClientProvider<ITuxExchangeApi>(TuxExchangeApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r?.Count > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r?.Keys == null || r.Keys.Count == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.Keys)
            {
                pairs.Add(rCurrentTicker.ToAssetPair(this));
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

            if (r?.Count == 0)
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
                    prices.Add(new MarketPrice(Network, pair, 1 / currentTicker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.lowestAsk, currentTicker.highestBid, currentTicker.low24hr, currentTicker.high24hr),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.baseVolume, currentTicker.quoteVolume)
                    });
                }
            }

            return prices;
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetVolumesAsync().ConfigureAwait(false);

            var rPairsDict = r.ToDictionary(x => x.Key.ToAssetPair(this), x => x.Value);

            var pairsQueryable = context.IsRequestAll ? rPairsDict.Keys.ToList() : context.Pairs;

            var volumes = new List<NetworkPairVolume>();
            var missingPairs = new List<AssetPair>();

            foreach (var pair in pairsQueryable)
            {
                if (!rPairsDict.TryGetValue(pair, out var volumeInfo))
                {
                    missingPairs.Add(pair);
                    continue;
                }

                var baseVolume = volumeInfo.FirstOrDefault(x => x.Key.Equals(pair.Asset1.ShortCode)).Value;
                var quoteVolume = volumeInfo.FirstOrDefault(x => x.Key.Equals(pair.Asset2.ShortCode)).Value;

                volumes.Add(new NetworkPairVolume(Network, pair, baseVolume, quoteVolume));
            }

            return new PublicVolumeResponse(Network, volumes, missingPairs);
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetOrderBookAsync(context.Pair.Asset1.ShortCode, context.Pair.Asset2.ShortCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair.Reversed);

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
