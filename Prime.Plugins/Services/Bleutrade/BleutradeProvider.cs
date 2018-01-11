using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;
using System.Linq;

namespace Prime.Plugins.Services.Bleutrade
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://bleutrade.com/help/API
    public class BleutradeProvider : IPublicPricingProvider, IAssetPairsProvider, IPublicVolumeProvider, IOrderBookProvider
    {
        private const string BleutradeApiVersion = "v2";
        private const string BleutradeApiUrl = "https://bleutrade.com/api/" + BleutradeApiVersion;

        private static readonly ObjectId IdHash = "prime:bleutrade".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBleutradeApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Bleutrade");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BleutradeProvider()
        {
            ApiProvider = new RestApiClientProvider<IBleutradeApi>(BleutradeApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetMarketsAsync().ConfigureAwait(false);

            return r.success && r.result.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetMarketsAsync().ConfigureAwait(false);

            if (r.success == false || r.result.Length == 0)
            {
                throw new ApiResponseException(r.message, this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r.result)
            {
                pairs.Add(rCurrentTicker.MarketName.ToAssetPair(this));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = false }
            //Commented-out for now, as even though the API has an example for this, it does NOT work on their end.
            //Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = false }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r.success == false && r.result.Length > 0)
            {
                throw new ApiResponseException(r.message, this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, 1 / r.result[0].Last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.result[0].Ask, r.result[0].Bid)
            });
        }

        public async Task<PublicVolumeResponse> GetVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this);

            var r = await api.GetMarketAsync(pairCode).ConfigureAwait(false);

            if (r.success == false && r.result.Length > 0)
            {
                throw new ApiResponseException(r.message, this);
            }

            return new PublicVolumeResponse(Network, context.Pair, r.result[0].BaseVolume, r.result[0].Volume);
        }

        public async Task<PublicVolumeResponse> GetVolumesAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetMarketsAsync().ConfigureAwait(false);

            if (r.success == false)
            {
                throw new ApiResponseException(r.message, this);
            }

            var rPairsDict = r.result.ToDictionary(x => x.MarketName.ToAssetPair(this), x => x);

            var pairsQueryable = context.IsRequestAll ? rPairsDict.Keys.ToList() : context.Pairs;

            var missing = new List<AssetPair>();
            var volumes = new List<NetworkPairVolume>();

            foreach (var pair in pairsQueryable)
            {
                if (!rPairsDict.TryGetValue(pair, out var ticker))
                {
                    missing.Add(pair);
                    continue;
                }

                volumes.Add(new NetworkPairVolume(Network, pair,ticker.BaseVolume, ticker.Volume));
            }

            return new PublicVolumeResponse(Network, volumes, missing);
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            if (context.ForSingleMethod)
                return await GetVolumeAsync(context).ConfigureAwait(false);

            return await GetVolumesAsync(context).ConfigureAwait(false);
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var asks = r.result.sell.Take(maxCount);
            var bids = r.result.buy.Take(maxCount);

            foreach (var i in bids)
                orderBook.AddBid(i.rate, i.quantity, true);

            foreach (var i in asks)
                orderBook.AddAsk(i.rate, i.quantity, true);

            return orderBook;
        }

        private static readonly VolumeFeatures StaticVolumeFeatures = new VolumeFeatures()
        {
            Single = new VolumeSingleFeatures() {  CanVolumeBase = true, CanVolumeQuote = true },
            Bulk = new VolumeBulkFeatures() { CanReturnAll = true, CanVolumeBase = true, CanVolumeQuote = true }
        };

        public VolumeFeatures VolumeFeatures => StaticVolumeFeatures;
    }
}
