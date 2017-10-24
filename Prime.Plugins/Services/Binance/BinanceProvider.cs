using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Plugins.Services.Binance
{
    public class BinanceProvider : IExchangeProvider, IOrderBookProvider, IWalletService, IOhlcProvider
    {
        // public const string BinanceApiVersion = "v1";
        public const string BinanceApiUrl = "https://www.binance.com/api";

        private static readonly ObjectId IdHash = "prime:bitflyer".GetObjectIdHashCode();

        private RestApiClientProvider<IBinanceApi> ApiProvider { get; }

        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("Binance");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;
        public bool CanMultiDepositAddress => false;
        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BinanceProvider()
        {
            ApiProvider = new RestApiClientProvider<IBinanceApi>(BinanceApiUrl, this, k => new BinanceAuthenticator(k).GetRequestModifier);
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerSimple();

            var interval = ConvertToBinanceInterval(context.Market);
            var startDate = (long)(context.Range.UtcFrom.ToUnixTimeStamp() * 1000);
            var endDate = (long)(context.Range.UtcTo.ToUnixTimeStamp() * 1000);

            var r = await api.GetCandlestickBars(pairCode, interval, startDate, endDate);

            var ohlc = new OhlcData(context.Market);

            var seriesId = OhlcResolutionAdapter.GetHash(context.Pair, context.Market, Network);

            foreach (var rEntry in r)
            {
                var dateTime = ((long) (rEntry[0] / 1000)).ToUtcDateTime();
                ohlc.Add(new OhlcEntry(seriesId, dateTime, this)
                {
                    Open = (double)rEntry[1],
                    Close = (double)rEntry[4],
                    Low = (double)rEntry[3],
                    High = (double)rEntry[2],
                    VolumeTo = (double)rEntry[7], // Quote asset volume
                    VolumeFrom = (double)rEntry[5], // Volume
                    WeightedAverage = 0 // BUG: no WeightedAverage data returned from API.
                });
            }

            ohlc.Reverse();

            return ohlc;
        }

        private string ConvertToBinanceInterval(TimeResolution resolution)
        {
            // BUG: API doesn't support seconds and milliseconds.
            switch (resolution)
            {
                case TimeResolution.Minute:
                    return "1m";
                case TimeResolution.Hour:
                    return "1h";
                case TimeResolution.Day:
                    return "1d";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<LatestPrice> GetPairPriceAsync(PublicPairPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetSymbolPriceTicker();

            var lowerPairTicker = context.Pair.TickerSimple().ToLower();

            var lpr = r.FirstOrDefault(x => x.symbol.ToLower().Equals(lowerPairTicker));

            if(lpr == null)
                throw new ApiResponseException("Specified currency pair is not supported by provider", this);

            var latestPrice = new LatestPrice()
            {
                Price = new Money(lpr.price, context.Pair.Asset2),
                BaseAsset = context.Pair.Asset1,
                UtcCreated = DateTime.UtcNow
            };

            return latestPrice;
        }

        public async Task<LatestPrices> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var moneyList = new List<Money>();

            var r = await api.GetSymbolPriceTicker();

            foreach (var asset in context.Assets)
            {
                var currentAssetPair = new AssetPair(context.QuoteAsset, asset);
                var rPrice = r.FirstOrDefault(x => x.symbol.Equals(currentAssetPair.TickerSimple()));

                if (rPrice == null)
                    continue;

                moneyList.Add(new Money(rPrice.price, asset));
            }

            var latestPrices = new LatestPrices()
            {
                BaseAsset = context.QuoteAsset,
                Prices = moneyList,
                UtcCreated = DateTime.UtcNow
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

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetSymbolPriceTicker();

            var assetPairs = new AssetPairs();

            foreach (var rPrice in r)
            {
                var pair = GetAssetPair(rPrice.symbol, assetPairs);

                if (pair == null)
                    continue;

                assetPairs.Add(pair);
            }

            return assetPairs;
        }

        private AssetPair GetAssetPair(string pairCode, AssetPairs existingPairs)
        {
            AssetPair assetPair = null;

            if (pairCode.Length == 6)
            {
                var asset1 = pairCode.Substring(0, 3);
                var asset2 = pairCode.Substring(3);

                assetPair = new AssetPair(asset1, asset2);
            }
            else if(pairCode.Length > 6)
            {
                var existingAsset = existingPairs.FirstOrDefault(x => pairCode.Contains(x.Asset1.ShortCode))?.Asset1;
                if (existingAsset != null)
                {
                    var asset1 = existingAsset.ShortCode;
                    var asset2 = pairCode.Replace(existingAsset.ShortCode, "");

                    assetPair = new AssetPair(asset1, asset2);
                }
                else
                {
                    existingAsset = existingPairs.FirstOrDefault(x => pairCode.Contains(x.Asset2.ShortCode))?.Asset2;
                    if (existingAsset != null)
                    {
                        var asset1 = pairCode.Replace(existingAsset.ShortCode, "");
                        var asset2 = existingAsset.ShortCode;

                        assetPair = new AssetPair(asset1, asset2);
                    }
                }
            }

            return assetPair;
        }

        public async Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            CheckOrderRecordsInputNumber(context.MaxRecordsCount);

            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.TickerSimple();

            var r = context.MaxRecordsCount.HasValue
                ? await api.GetOrderBook(pairCode, context.MaxRecordsCount.Value / 2)
                : await api.GetOrderBook(pairCode);

            var orderBook = new OrderBook();

            foreach (var rBid in r.asks)
            {
                var bid = GetOrderBookRecordData(rBid);

                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Bid,
                    Data = new BidAskData()
                    {
                        Time = DateTime.UtcNow,
                        Price = new Money(bid.Price, context.Pair.Asset2),
                        Volume = bid.Volume
                    }
                });
            }

            foreach (var rAsk in r.asks)
            {
                var ask = GetOrderBookRecordData(rAsk);

                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Ask,
                    Data = new BidAskData()
                    {
                        Time = DateTime.UtcNow,
                        Price = new Money(ask.Price, context.Pair.Asset2),
                        Volume = ask.Volume
                    }
                });
            }

            return orderBook;
        }

        private (decimal Price, decimal Volume) GetOrderBookRecordData(object[] rawEntry)
        {
            if(!decimal.TryParse(rawEntry[0].ToString(), out var price))
                throw new ApiResponseException("Incorrect format of returned price", this);

            if(!decimal.TryParse(rawEntry[1].ToString(), out var volume))
                throw new ApiResponseException("Incorrect format of returned volume", this);

            return (price, volume);
        }

        private void CheckOrderRecordsInputNumber(int? recordsNumber)
        {
            var validRecordsCount = new int[] { 5, 10, 20, 50, 100, 200, 500 };

            if (recordsNumber.HasValue && !validRecordsCount.Contains(recordsNumber.Value))
                throw new ArgumentException("Specified number of order book records is not supported");
        }

        public Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountInformation();

            return r != null;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountInformation();

            var balances = new BalanceResults();

            foreach (var b in r.balances)
            {
                var asset = b.asset.ToAsset(this);

                balances.Add(new BalanceResult(asset)
                {
                    Available = new Money(b.free, asset),
                    Balance = new Money(b.free, asset),
                    Reserved = new Money(b.locked, asset)
                });
            }

            return balances;
        }
    }
}
