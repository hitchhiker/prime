using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Plugins.Services.Base;
using Prime.Utility;

namespace Prime.Plugins.Services.Korbit
{
    public class KorbitProvider : IExchangeProvider, IOrderBookProvider, IWalletService
    {
        private static readonly ObjectId IdHash = "prime:korbit".GetObjectIdHashCode();
        private static readonly string _pairs = "btckrw,etckrw,ethkrw,xrpkrw";
        
        public const string KorbitApiVersion = "v1/";
        public const string KorbitApiUrl = "https://api.korbit.co.kr/" + KorbitApiVersion;

        private RestApiClientProvider<IKorbitApi> ApiProvider { get; }

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);
        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("Korbit");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        // https://apidocs.korbit.co.kr/#first_section
        // ... Ticker calls are limited to 60 calls per 60 seconds. ...
        public IRateLimiter RateLimiter { get; } = new PerMinuteRateLimiter(60, 1);
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public bool CanMultiDepositAddress => false;
        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;

        public KorbitProvider()
        {
            ApiProvider = new RestApiClientProvider<IKorbitApi>(KorbitApiUrl, this, k => new KorbitAuthenticator(k).GetRequestModifier);
        }

        public async Task<LatestPrice> GetPairPriceAsync(PublicPairPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = GetKorbitTicker(context.Pair);

            var r = await api.GetTicker(pairCode);

            var sTimeStamp = r.timestamp / 1000; // r.timestamp is returned in ms.

            var latestPrice = new LatestPrice()
            {
                BaseAsset = context.Pair.Asset1,
                UtcCreated = sTimeStamp.ToUtcDateTime(),
                Price = new Money(r.last, context.Pair.Asset2)
            };

            return latestPrice;
        }

        public async Task<LatestPrices> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var moneyList = new List<Money>();

            foreach (var asset in context.Assets)
            {
                var pair = new AssetPair(context.QuoteAsset, asset);
                var pairCode = GetKorbitTicker(pair);

                var r = await api.GetTicker(pairCode);

                moneyList.Add(new Money(r.last, asset));

                ApiHelpers.EnterRate(this, context);
            }

            var latestPrices = new LatestPrices()
            {
                UtcCreated = DateTime.UtcNow,
                BaseAsset = context.QuoteAsset,
                Prices = moneyList
            };

            return latestPrices;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            throw new NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new NotImplementedException();
        }

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var t = new Task<AssetPairs>(() => Pairs);
            t.RunSynchronously();

            return t;
        }

        public async Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = GetKorbitTicker(context.Pair);

            var r = await api.GetOrderBook(pairCode);

            var bids = context.MaxRecordsCount.HasValue 
                ? r.bids.Take(context.MaxRecordsCount.Value / 2) 
                : r.bids;

            var asks = context.MaxRecordsCount.HasValue
                ? r.asks.Take(context.MaxRecordsCount.Value / 2)
                : r.asks;

            var orderBook = new OrderBook();

            var dateTime = (r.timestamp / 1000).ToUtcDateTime();

            foreach (var rBid in bids)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Bid,
                    Data = new BidAskData()
                    {
                        Price = new Money(rBid[0], context.Pair.Asset2),
                        Time = dateTime,
                        Volume = rBid[1]
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
                        Price = new Money(rAsk[0], context.Pair.Asset2),
                        Time = dateTime,
                        Volume = rAsk[1]
                    }
                });
            }

            return orderBook;
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

        public Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context)
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

        private string GetKorbitTicker(AssetPair pair)
        {
            return new AssetPair(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this)).TickerUnderslash().ToLower();
        }
    }
}
