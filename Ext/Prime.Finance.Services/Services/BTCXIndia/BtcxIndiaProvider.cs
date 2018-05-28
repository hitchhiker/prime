﻿using System;
using System.Threading.Tasks;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Services.Services.BTCXIndia
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://m.btcxindia.com/api/
    public class BtcxIndiaProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        public Version Version { get; } = new Version(1, 0, 0);
        private const string BtcxIndiaApiUrl = "https://m.btcxindia.com/api/";

        private static readonly ObjectId IdHash = "prime:btcxindia".GetObjectIdHashCode();

        //Nothing mentioned in documentation.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBtcxIndiaApi> ApiProvider { get; }
        private const string PairsCsv = "btcinr";

        public Network Network { get; } = Networks.I.Get("BTCXIndia");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public char? CommonPairSeparator => null;

        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BtcxIndiaProvider()
        {
            ApiProvider = new RestApiClientProvider<IBtcxIndiaApi>(BtcxIndiaApiUrl, this, (k) => null);
        }

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

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
            var r = await api.GetTickersAsync().ConfigureAwait(false);
            
            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last_traded_price)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.total_volume_24h)
            });
        }
    }
}
