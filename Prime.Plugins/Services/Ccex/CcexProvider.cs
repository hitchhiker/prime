using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Ccex
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://c-cex.com/?id=api
    public class CcexProvider : IPublicPricingProvider, IAssetPairsProvider, IPublicVolumeProvider
    {
        private const string CcexApiUrl = "https://c-cex.com/";

        private static readonly ObjectId IdHash = "prime:ccex".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<ICcexApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Ccex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '-';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public CcexProvider()
        {
            ApiProvider = new RestApiClientProvider<ICcexApi>(CcexApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            return r?.pairs?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            if (r?.pairs == null || r.pairs.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var pair in r.pairs)
            {
                pairs.Add(pair.ToAssetPair(this));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = false },
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = false, CanReturnAll = true }
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
            var pairCode = context.Pair.ToTicker(this).ToLower();
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r?.ticker == null)
            {
                throw new ApiResponseException("No ticker returned", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.ticker.lastprice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ticker.sell, r.ticker.buy, r.ticker.low, r.ticker.high)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
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
                    prices.Add(new MarketPrice(Network, pair, currentTicker.lastprice)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.sell, currentTicker.buy, currentTicker.low, currentTicker.high)
                    });
                }
            }

            return prices;
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetVolumesAsync().ConfigureAwait(false);

            var rPairsDict = r.result.ToDictionary(x => x.MarketName.ToAssetPair(this), x => x);

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

                volumes.Add(new NetworkPairVolume(Network, pair, volumeInfo.BaseVolume, volumeInfo.Volume));
            }

            return new PublicVolumeResponse(Network, volumes, missingPairs);
        }

        private static readonly VolumeFeatures StaticVolumeFeatures = new VolumeFeatures()
        {
            Bulk = new VolumeBulkFeatures() { CanReturnAll = true, CanVolumeBase = true, CanVolumeQuote = true }
        };

        public VolumeFeatures VolumeFeatures => StaticVolumeFeatures;
    }
}
