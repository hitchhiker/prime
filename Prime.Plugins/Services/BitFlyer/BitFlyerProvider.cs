using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BitFlyer
{
    // https://lightning.bitflyer.jp/docs
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public class BitFlyerProvider : IOrderBookProvider, IPublicPricingProvider, IAssetPairsProvider
    {
        public const string BitFlyerApiUrl = "https://api.bitflyer.com/" + BitFlyerApiVersion;
        public const string BitFlyerApiVersion = "v1";

        private static readonly ObjectId IdHash = "prime:bitflyer".GetObjectIdHashCode();

        private RestApiClientProvider<IBitFlyerApi> ApiProvider { get; }

        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("BitFlyer");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;
        public char? CommonPairSeparator { get; }

        // Each IP address is limited to approx. 500 queries per minute.
        // https://lightning.bitflyer.jp/docs?lang=en#api-limits
        public IRateLimiter RateLimiter { get; } = new PerMinuteRateLimiter(400, 1);

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        
        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;

        public BitFlyerProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitFlyerApi>(BitFlyerApiUrl, this, k => new BitFlyerAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var pairs = await GetAssetPairsAsync(context).ConfigureAwait(false);

            return pairs.Count > 0;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() {CanStatistics = true, CanVolume = true}
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var productCode = context.Pair.ToTicker(this, '_');

            var r = await api.GetTickerAsync(productCode).ConfigureAwait(false);

            var price = new MarketPrice(Network, context.Pair, r.ltp)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.best_ask, r.best_bid),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume_by_product)
            };

            return new MarketPrices(price);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, '_');

            var r = await api.GetBoardAsync(pairCode).ConfigureAwait(false);

            var bids = context.MaxRecordsCount == Int32.MaxValue
                ? r.bids
                : r.bids.Take(context.MaxRecordsCount);

            var asks = context.MaxRecordsCount == Int32.MaxValue
                ? r.asks
                : r.asks.Take(context.MaxRecordsCount);

            var orderBook = new OrderBook(Network, context.Pair);

            foreach (var i in bids)
                orderBook.AddBid(i.price, i.size);
            
            foreach (var i in asks)
                orderBook.AddAsk(i.price, i.size);

            return orderBook;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var assetPairs = new AssetPairs();

            var r = await api.GetMarketsAsync().ConfigureAwait(false);

            foreach (var rMarket in r)
            {
                var pieces = rMarket.product_code.Split('_');
                if (pieces.Length != 2)
                    continue;

                assetPairs.Add(rMarket.product_code.ToAssetPair(this, '_'));
            }

            return assetPairs;
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var productCode = context.Pair.ToTicker(this, '_');

            var r = await api.GetTickerAsync(productCode).ConfigureAwait(false);

            return new PublicVolumeResponse(Network, context.Pair, r.volume);
        }

        public VolumeFeatures VolumeFeatures { get; }
    }
}
