using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Plugins.Services.BitFlyer
{
    public class BitFlyerProvider : IOrderBookProvider, IPublicPriceProvider, IAssetPairsProvider, IPublicPriceStatistics
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

        // Each IP address is limited to approx. 500 queries per minute.
        // https://lightning.bitflyer.jp/docs?lang=en#api-limits
        public IRateLimiter RateLimiter { get; } = new PerMinuteRateLimiter(400, 1);

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        
        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;

        public BitFlyerProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitFlyerApi>(BitFlyerApiUrl, this, k => new BitFlyerAuthenticator(k).GetRequestModifier);
        }

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var productCode = GetBitFlyerTicker(context.Pair);

            var r = await api.GetTicker(productCode).ConfigureAwait(false);

            return new MarketPrice(Network, context.Pair, r.ltp)
            {
                PriceStatistics = new PriceStatistics(context.Pair.Asset2, r.volume, null, r.best_ask, r.best_bid, null, null)
            };
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = GetBitFlyerTicker(context.Pair);

            var r = await api.GetBoard(pairCode).ConfigureAwait(false);

            var bids = context.MaxRecordsCount.HasValue
                ? r.bids.Take(context.MaxRecordsCount.Value / 2)
                : r.bids;

            var asks = context.MaxRecordsCount.HasValue
                ? r.asks.Take(context.MaxRecordsCount.Value / 2)
                : r.asks;

            var orderBook = new OrderBook();

            foreach (var rBid in bids)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Bid,
                    Data = new BidAskData()
                    {
                        Time = DateTime.UtcNow,
                        Price = new Money(rBid.price, context.Pair.Asset2),
                        Volume = rBid.size
                    }
                });
            }

            foreach (var rAsk in asks)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Ask,
                    Data = new BidAskData()
                    {
                        Time = DateTime.UtcNow,
                        Price = new Money(rAsk.price, context.Pair.Asset2),
                        Volume = rAsk.size
                    }
                });
            }

            return orderBook;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var assetPairs = new AssetPairs();

            var r = await api.GetMarkets().ConfigureAwait(false);

            foreach (var rMarket in r)
            {
                var pieces = rMarket.product_code.Split('_');
                if (pieces.Length != 2)
                    continue;

                assetPairs.Add(rMarket.product_code.ToAssetPair(this));
            }

            return assetPairs;
        }

        private string GetBitFlyerTicker(AssetPair pair)
        {
            return new AssetPair(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this)).TickerUnderslash();
        }

        public async Task<VolumeResult> GetVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);
            var productCode = GetBitFlyerTicker(context.Pair);

            var r = await api.GetTicker(productCode).ConfigureAwait(false);

            return new VolumeResult()
            {
                Pair = context.Pair,
                Volume = r.volume,
                Period = VolumePeriod.Day
            };
        }
    }
}
