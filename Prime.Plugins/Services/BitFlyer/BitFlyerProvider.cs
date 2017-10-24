using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Plugins.Services.BitFlyer
{
    public class BitFlyerProvider : IWalletService, IOrderBookProvider, IExchangeProvider
    {
        public const string BitFlyerApiUrl = "https://api.bitflyer.com/" + BitFlyerApiVersion;
        public const string BitFlyerApiVersion = "v1";

        private static readonly ObjectId IdHash = "prime:bitflyer".GetObjectIdHashCode();

        private RestApiClientProvider<IBitFlyerApi> ApiProvider { get; }

        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("BitFlyer");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        // Each IP address is limited to approx. 500 queries per minute.
        // https://lightning.bitflyer.jp/docs?lang=en#api-limits
        public IRateLimiter RateLimiter { get; } = new PerMinuteRateLimiter(400, 1);

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public bool CanMultiDepositAddress => false;
        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;

        public BitFlyerProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitFlyerApi>(BitFlyerApiUrl, this, k => new BitFlyerAuthenticator(k).GetRequestModifier);
        }

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var productCode = GetBitFlyerTicker(context.Pair);

            var r = await api.GetTicker(productCode);

            var latestPrice = new LatestPrice()
            {
                Price = new Money(r.ltp, context.Pair.Asset2),
                BaseAsset = context.Pair.Asset1,
                UtcCreated = DateTime.UtcNow
            };

            return latestPrice;
        }

        public async Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var moneyList = new List<Money>();

            foreach (var asset in context.Assets)
            {
                var pair = new AssetPair(context.BaseAsset, asset);
                var pairCode = GetBitFlyerTicker(pair);

                var r = await api.GetTicker(pairCode);

                moneyList.Add(new Money(r.ltp, asset));
            }

            var latestPrices = new LatestPrices()
            {
                BaseAsset = context.BaseAsset,
                Prices = moneyList,
                UtcCreated = DateTime.UtcNow
            };

            return latestPrices;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TestApiAsync(ApiTestContext context)
        {
            throw new NotImplementedException();
        }

        public Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = GetBitFlyerTicker(context.Pair);

            var r = await api.GetBoard(pairCode);

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

        public BuyResult Buy(BuyContext ctx)
        {
            throw new NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new NotImplementedException();
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var assetPairs = new AssetPairs();

            var r = await api.GetMarkets();

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
    }
}
