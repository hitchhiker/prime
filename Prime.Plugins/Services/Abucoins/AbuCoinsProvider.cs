﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Abucoins
{
    // https://docs.abucoins.com
    public class AbucoinsProvider : IPublicPricingProvider, IAssetPairsProvider
    {
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
        public char? CommonPairSeparator { get; }

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
                throw new ApiResponseException("No asset pairs returned.", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r)
            {
                pairs.Add(rCurrentTicker.id.ToAssetPair(this, '-'));
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

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, '-');
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new MarketPricesResult(new MarketPrice(Network, context.Pair, r.price)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, null, null),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }
    }
}
