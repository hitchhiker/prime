using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Plugins.Services.Poloniex
{
    public class PoloniexProvider : IExchangeProvider, IWalletService, IOhlcProvider, IOrderBookProvider, IPublicPricesProvider
    {
        private const String PoloniexApiUrl = "https://poloniex.com";

        private RestApiClientProvider<IPoloniexApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Poloniex");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        private static readonly ObjectId IdHash = "prime:poloniex".GetObjectIdHashCode();
        public ObjectId Id => IdHash;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;


        public bool CanMultiDepositAddress => false;
        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => true;

        public PoloniexProvider()
        {
            ApiProvider = new RestApiClientProvider<IPoloniexApi>(PoloniexApiUrl, this, k => new PoloniexAuthenticator(k).GetRequestModifier);
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreatePoloniexBody(PoloniexBodyType.ReturnBalances);

            try
            {
                var r = await api.GetBalancesAsync(body);

                return r != null && r.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickerAsync();

            var assetPairsInfo = r.Where(x => x.Key.ToAssetPair(this).Equals(context.Pair)).ToList();

            if (assetPairsInfo.Count < 1)
                throw new ApiResponseException("Specified asset pair is not supported by this API", this);

            var selectedPair = assetPairsInfo[0];

            return new MarketPrice(context.Pair, 1 / selectedPair.Value.last);
        }

        public async Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return await GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync();

            var prices = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var rTickers = r.Where(x => x.Key.ToAssetPair(this).Equals(pair)).ToList();

                if (rTickers.Count == 0)
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                var rTicker = rTickers[0];

                prices.MarketPrices.Add(new MarketPrice(pair, 1 / rTicker.Value.last));
            }

            return prices;
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
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickerAsync();

            var pairs = new AssetPairs();

            foreach (var rPair in r)
            {
                var pair = rPair.Key.ToAssetPair(this);

                pairs.Add(pair);
            }

            return pairs;
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreatePoloniexBody(PoloniexBodyType.ReturnDepositAddresses);

            var addresses = new WalletAddresses();

            try
            {
                var r = await api.GetDepositAddressesAsync(body);

                foreach (var balance in r)
                {
                    if (string.IsNullOrWhiteSpace(balance.Value))
                        continue;

                    addresses.Add(new WalletAddress(this, balance.Key.ToAsset(this)) { Address = balance.Value });
                }
            }
            catch(Exception e)
            {
                // "Unable to get deposit addresses, please check that your account is verified"
                throw new ApiResponseException(e, this);
            }

            return addresses;
        }

        public Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreatePoloniexBody(PoloniexBodyType.ReturnCompleteBalances);

            var r = await api.GetBalancesDetailedAsync(body);

            var results = new BalanceResults(this);

            foreach (var kvp in r)
            {
                var c = kvp.Key.ToAsset(this);

                results.Add(new BalanceResult(c)
                {
                    Available = new Money(kvp.Value.available, c),
                    Reserved = new Money(kvp.Value.onOrders, c),
                    Balance = new Money(kvp.Value.available, c)
                });
            }

            return results;
        }

        private Dictionary<string, object> CreatePoloniexBody(PoloniexBodyType bodyType)
        {
            var body = new Dictionary<string, object> {{"nonce", BaseAuthenticator.GetLongNonce()}};

            switch (bodyType)
            {
                case PoloniexBodyType.ReturnBalances:
                    body.Add("command", "returnBalances");
                    break;
                case PoloniexBodyType.ReturnCompleteBalances:
                    body.Add("command", "returnCompleteBalances");
                    break;
                case PoloniexBodyType.ReturnDepositAddresses:
                    body.Add("command", "returnDepositAddresses");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null);
            }

            return body;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreatePoloniexBody(PoloniexBodyType.ReturnDepositAddresses);

            var addresses = new WalletAddresses();

            try
            {
                var r = await api.GetDepositAddressesAsync(body);
                var assetBalances = r.Where(x => Equals(x.Key.ToAsset(this), context.Asset)).ToArray();

                foreach (var balance in assetBalances)
                {
                    if (string.IsNullOrWhiteSpace(balance.Value))
                        continue;

                    addresses.Add(new WalletAddress(this, balance.Key.ToAsset(this)) { Address = balance.Value });
                }
            }
            catch (Exception e)
            {
                // "Unable to get deposit addresses, please check that your account is verified"
                throw new ApiResponseException(e, this);
            }

            return addresses;
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var pair = context.Pair;
            var market = context.Market;

            var timeStampStart = (long)context.Range.UtcFrom.ToUnixTimeStamp();
            var timeStampEnd = (long)context.Range.UtcTo.ToUnixTimeStamp();

            var period = ConvertToPoloniexInterval(market);

            var api = ApiProvider.GetApi(context);
            var r = await api.GetChartDataAsync(pair.TickerUnderslash(), timeStampStart, timeStampEnd, period);

            var ohlc = new OhlcData(market);
            var seriesid = OhlcUtilities.GetHash(pair, market, Network);

            foreach (var ohlcEntry in r)
            {
                ohlc.Add(new OhlcEntry(seriesid, ohlcEntry.date.ToUtcDateTime(), this)
                {
                    Open = ohlcEntry.open,
                    Close = ohlcEntry.close,
                    Low = ohlcEntry.low,
                    High = ohlcEntry.high,
                    VolumeTo = ohlcEntry.quoteVolume,
                    VolumeFrom = ohlcEntry.volume,
                    WeightedAverage = ohlcEntry.weightedAverage
                });
            }

            return ohlc;
        }

        private PoloniexTimeInterval ConvertToPoloniexInterval(TimeResolution resolution)
        {
            // TODO: implement all TimeResolution cases.
            switch (resolution)
            {
                case TimeResolution.Day:
                    return PoloniexTimeInterval.Day1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        public async Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.TickerUnderslash();

            var r = context.MaxRecordsCount.HasValue ? await api.GetOrderBook(pairCode, context.MaxRecordsCount.Value / 2) : await api.GetOrderBook(pairCode);

            if (r.bids == null || r.asks == null)
                throw new ApiResponseException("Specified currency is not supported", this);

            var orderBook = new OrderBook();

            foreach (var rBid in r.bids)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData()
                    {
                        Price = new Money(rBid[0], context.Pair.Asset2),
                        Time = DateTime.UtcNow,
                        Volume = rBid[1]
                    },
                    Type = OrderBookType.Bid
                });
            }

            foreach (var rAsk in r.asks)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData()
                    {
                        Price = new Money(rAsk[0], context.Pair.Asset2),
                        Time = DateTime.UtcNow,
                        Volume = rAsk[1]
                    },
                    Type = OrderBookType.Ask
                });
            }

            return orderBook;
        }
    }
}
