using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using plugins;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Utility;
using RestEase;
using OrderBook = Prime.Common.OrderBook;

namespace Prime.Plugins.Services.Coinbase
{
    public class CoinbaseProvider : IExchangeProvider, IWalletService, IOrderBookProvider, IOhlcProvider
    {
        private static readonly ObjectId IdHash = "prime:coinbase".GetObjectIdHashCode();

        private const string CoinbaseApiVersion = "v2";
        private const string CoinbaseApiUrl = "https://api.coinbase.com/" + CoinbaseApiVersion + "/";

        private const string GdaxApiUrl = "https://api.gdax.com";

        private RestApiClientProvider<ICoinbaseApi> ApiProvider { get; }
        private RestApiClientProvider<IGdaxApi> GdaxApiProvider { get; }

        public Network Network { get; } = new Network("Coinbase");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;

        // 10000 per hour.
        // https://developers.coinbase.com/api/v2#rate-limiting
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(2, 1);
        public IRateLimiter RateLimiter => Limiter;

        public bool CanMultiDepositAddress => true;
        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => true;

        public CoinbaseProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinbaseApi>(CoinbaseApiUrl, this, k => new CoinbaseAuthenticator(k).GetRequestModifier);
            GdaxApiProvider = new RestApiClientProvider<IGdaxApi>(GdaxApiUrl);
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public async Task<LatestPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = GetCoinbaseTicker(context.Pair.Asset1, context.Pair.Asset2);
            var r = await api.GetLatestPrice(pairCode);

            var price = new LatestPrice(new Money(r.data.amount, r.data.currency.ToAsset(this)), context.QuoteAsset)
            {
                QuoteAsset = context.Pair.Asset1
            };

            return price;
        }

        private string GetCoinbaseTicker(Asset baseAsset, Asset asset)
        {
            return new AssetPair(baseAsset.ToRemoteCode(this), asset.ToRemoteCode(this)).TickerDash();
        }

        public BuyResult Buy(BuyContext ctx)
        {
            return null;
        }

        public SellResult Sell(SellContext ctx)
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = GdaxApiProvider.GetApi(context);
            var r = await api.GetProducts();

            var pairs = new AssetPairs();

            foreach (var rProduct in r)
            {
                pairs.Add(new AssetPair(rProduct.base_currency, rProduct.quote_currency));
            }

            return pairs;
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountsAsync();
            return r != null;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountsAsync();

            var results = new BalanceResults(this);

            foreach (var a in r.data)
            {
                if (a.balance == null)
                    continue;

                var c = a.balance.currency.ToAsset(this);
                results.AddBalance(c, a.balance.amount);
                results.AddAvailable(c, a.balance.amount);
                results.AddReserved(c, a.balance.amount);
            }

            return results;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var accid = "";

            var accs = await api.GetAccounts();
            var ast = context.Asset.ToRemoteCode(this);

            var acc = accs.data.FirstOrDefault(x => string.Equals(x.currency, ast, StringComparison.OrdinalIgnoreCase));
            if (acc == null)
                return null;

            accid = acc.id;

            if (accid == null)
                return null;

            var r = await api.GetAddressesAsync(acc.id);
            // TODO: re-implement.
            //if (r.data.Count == 0 && context.CanGenerateAddress)
            //{
            //    var cr = await api.CreateAddressAsync(accid);
            //    if (cr != null)
            //        r.data.AddRange(cr.data);
            //}

            var addresses = new WalletAddresses();

            foreach (var a in r.data)
            {
                if (string.IsNullOrWhiteSpace(a.address))
                    continue;

                var forasset = FromNetwork(a.network);
                if (!context.Asset.Equals(forasset))
                    continue;

                addresses.Add(new WalletAddress(this, context.Asset) { Address = a.address });
            }

            return addresses;
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);
            var accs = await api.GetAccounts();
            var addresses = new WalletAddresses();

            var accountIds = accs.data.Select(x => new KeyValuePair<string, string>(x.currency, x.id));

            foreach (var kvp in accountIds)
            {
                var r = await api.GetAddressesAsync(kvp.Value);

                foreach (var rAddress in r.data)
                {
                    if(string.IsNullOrWhiteSpace(rAddress.address))
                        continue;

                    addresses.Add(new WalletAddress(this, kvp.Key.ToAsset(this))
                    {
                        Address = rAddress.address
                    });
                }
            }

            return addresses;
        }

        public Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        private Asset FromNetwork(string network)
        {
            switch (network)
            {
                case "bitcoin":
                    return "BTC".ToAssetRaw();
                case "litecoin":
                    return "LTC".ToAssetRaw();
                case "ethereum":
                    return "ETH".ToAssetRaw();
                default:
                    return Asset.None;
            }
        }

        public async Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            var api = GdaxApiProvider.GetApi(context);
            var pairCode = context.Pair.TickerDash();

            // TODO: Check this! Can we use limit when we query all records?
            var recordsLimit = 1000;

            var r = await api.GetProductOrderBook(pairCode, OrderBookDepthLevel.FullNonAggregated);

            var bids = context.MaxRecordsCount.HasValue 
                ? r.bids.Take(context.MaxRecordsCount.Value / 2).ToArray() 
                : r.bids.Take(recordsLimit).ToArray();
            var asks = context.MaxRecordsCount.HasValue
                ? r.asks.Take(context.MaxRecordsCount.Value / 2).ToArray()
                : r.asks.Take(recordsLimit).ToArray();

            var orderBook = new OrderBook();

            foreach (var rBid in bids)
            {
                var bid = ConvertToOrderBookRecord(rBid);

                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData()
                    {
                        Price = new Money(bid.Price, context.Pair.Asset2),
                        Time = DateTime.UtcNow,
                        Volume = bid.Size
                    },
                    Type = OrderBookType.Bid
                });
            }

            foreach (var rAsk in asks)
            {
                var ask = ConvertToOrderBookRecord(rAsk);

                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData()
                    {
                        Price = new Money(ask.Price, context.Pair.Asset2),
                        Time = DateTime.UtcNow,
                        Volume = ask.Size
                    },
                    Type = OrderBookType.Ask
                });
            }

            return orderBook;
        }

        private (decimal Price, decimal Size) ConvertToOrderBookRecord(string[] data)
        {
            if(!decimal.TryParse(data[0], out var price) || !decimal.TryParse(data[1], out var size))
                throw new ApiResponseException("API returned incorrect format of price data", this);

            return (price, size);
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = GdaxApiProvider.GetApi(context);
            var currencyCode = context.Pair.TickerDash();

            var ohlc = new OhlcData(context.Market);
            var seriesId = OhlcUtilities.GetHash(context.Pair, context.Market, Network);

            var granularitySeconds = GetSeconds(context.Market);
            var maxNumberOfCandles = 200;

            var tsFrom = (long)context.Range.UtcFrom.ToUnixTimeStamp();
            var tsTo = (long)context.Range.UtcTo.ToUnixTimeStamp();
            var tsStep = maxNumberOfCandles * granularitySeconds;

            var currTsTo = tsTo;
            var currTsFrom = tsTo - tsStep;

            while (currTsTo > tsFrom)
            {
                var candles = await api.GetCandles(currencyCode, currTsFrom.ToUtcDateTime(), currTsTo.ToUtcDateTime(), granularitySeconds);

                foreach (var candle in candles)
                {
                    var dateTime = ((long)candle[0]).ToUtcDateTime();
                    ohlc.Add(new OhlcEntry(seriesId, dateTime, this)
                    {
                        Low = (double)candle[1],
                        High = (double)candle[2],
                        Open = (double)candle[3],
                        Close = (double)candle[4],
                        VolumeTo = (long)candle[5],
                        VolumeFrom = (long)candle[5],
                        WeightedAverage = 0 // Is not provided by API.
                    });
                }

                currTsTo = currTsFrom;

                if (currTsTo - tsStep >= tsFrom)
                    currTsFrom -= tsStep;
                else
                    currTsFrom = tsFrom;

                ApiHelpers.EnterRate(this, context);
            }

            return ohlc;
        }

        private int GetSeconds(TimeResolution market)
        {
            switch (market)
            {
                case TimeResolution.Second:
                    return 1;
                case TimeResolution.Minute:
                    return 60;
                case TimeResolution.Hour:
                    return 3600;
                case TimeResolution.Day:
                    return 62400;
                default:
                    throw new ArgumentOutOfRangeException(nameof(market), market, null);
            }
        }
    }
}