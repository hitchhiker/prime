﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Services.Services.Gate
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://gate.io/api2
    public partial class GateProvider : IPublicPricingProvider, IAssetPairsProvider, IPublicVolumeProvider, IOrderBookProvider, INetworkProviderPrivate
    {
        public Version Version { get; } = new Version(1, 0, 0);
        private const string GateApiVersion = "api2";
        private const string GateApiUrl = "http://data.gate.io/" + GateApiVersion;
        private const string GateApiUrlPrivate = "https://api.gate.io/" + GateApiVersion;

        private static readonly ObjectId IdHash = "prime:gate".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IGateApi> ApiProvider { get; }
        private RestApiClientProvider<IGateApi> ApiPrivateProvider { get; }

        public Network Network { get; } = Networks.I.Get("Gate");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public GateProvider()
        {
            ApiPrivateProvider = new RestApiClientProvider<IGateApi>(GateApiUrl, this, (k) => null);
            ApiPrivateProvider = new RestApiClientProvider<IGateApi>(GateApiUrlPrivate, this, (k) => new GateAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            return r?.Length > 0;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiPrivateProvider.GetApi(context);

            var rRaw = await api.GetBalancesAsync().ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.result;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            if (r.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r)
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
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.lowestAsk, r.highestBid, r.low24hr, r.high24hr),
                Volume = new NetworkPairVolume(Network, context.Pair, r.baseVolume, r.quoteVolume)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
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
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last)
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

            if (r.result == false)
            {
                throw new ApiResponseException("No market list returned.", this);
            }

            var rPairsDict = r.data.ToDictionary(x => x.pair.ToAssetPair(this), x => x);

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

                volumes.Add(new NetworkPairVolume(Network, pair,volumeInfo.vol_a, volumeInfo.vol_b));
            }

            return new PublicVolumeResponse(Network, volumes, missingPairs);
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
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
