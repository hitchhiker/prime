using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Plugins.Services.Binance
{
    // https://www.binance.com/restapipub.html
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class BinanceProvider : IOrderBookProvider, IOhlcProvider, IPublicPricingProvider, IAssetPairsProvider, IDepositProvider
    {
        // public const string BinanceApiVersion = "v1";
        public const string BinanceApiUrl = "https://www.binance.com";

        private static readonly ObjectId IdHash = "prime:binance".GetObjectIdHashCode();

        private static readonly IReadOnlyList<Asset> SuspendedDeposit = "BTM,HCC,LLT,BTG".ToAssetsCsvRaw();
        private static readonly IReadOnlyList<Asset> SuspendedWithdrawal = "BTG".ToAssetsCsvRaw();

        private RestApiClientProvider<IBinanceApi> ApiProvider { get; }

        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("Binance");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;
        
        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;

        private static readonly BinanceCodeConverter AssetCodeConverter = new BinanceCodeConverter();
        public IAssetCodeConverter GetAssetCodeConverter() => AssetCodeConverter;

        public Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context)
        {
            return Task.FromResult(new TransferSuspensions(SuspendedDeposit, SuspendedWithdrawal));
        }

        public Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            return Task.FromResult<WalletAddresses>(null);
        }

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            return Task.FromResult<WalletAddresses>(null);
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BinanceProvider()
        {
            ApiProvider = new RestApiClientProvider<IBinanceApi>(BinanceApiUrl, this, k => new BinanceAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi();

            var rRaw = await api.PingAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            return true;
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this);

            var interval = ConvertToBinanceInterval(context.Market);
            var startDate = (long)(context.Range.UtcFrom.ToUnixTimeStamp() * 1000);
            var endDate = (long)(context.Range.UtcTo.ToUnixTimeStamp() * 1000);

            var rRaw = await api.GetCandlestickBarsAsync(pairCode, interval, startDate, endDate).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var ohlc = new OhlcData(context.Market);

            var seriesId = OhlcUtilities.GetHash(context.Pair, context.Market, Network);

            foreach (var rEntry in r)
            {
                var dateTime = ((long)(rEntry[0] / 1000)).ToUtcDateTime();
                ohlc.Add(new OhlcEntry(seriesId, dateTime, this)
                {
                    Open = rEntry[1],
                    Close = rEntry[4],
                    Low = rEntry[3],
                    High = rEntry[2],
                    VolumeTo = rEntry[7], // Quote asset volume
                    VolumeFrom = rEntry[5], // Volume
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

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true},
            Bulk = new PricingBulkFeatures() { CanReturnAll = true }
        };
        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetSymbolPriceTickerAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var prices = new MarketPrices();
            var knownPairs = new AssetPairs();

            if (context.IsRequestAll)
            {
                foreach (var rPrice in r.OrderBy(x => x.symbol.Length))
                {
                    var tPair = AssetsUtilities.GetAssetPair(rPrice.symbol, knownPairs);

                    if (!tPair.HasValue)
                        continue;

                    var pair = new AssetPair(tPair.Value.AssetCode1, tPair.Value.AssetCode2, this);

                    knownPairs.Add(pair);

                    prices.Add(new MarketPrice(Network, pair, rPrice.price));
                }
            }
            else
            {
                foreach (var pair in context.Pairs)
                {
                    var lowerPairTicker = pair.ToTicker(this).ToLower();

                    var lpr = r.FirstOrDefault(x => x.symbol.ToLower().Equals(lowerPairTicker));

                    if (lpr == null)
                    {
                        prices.MissedPairs.Add(pair);
                        continue;
                    }

                    prices.Add(new MarketPrice(Network, pair, lpr.price));
                }
            }

            return prices;
        }

        public async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var ticker = context.Pair.ToTicker(this);
            var rRaw = await api.Get24HrTickerAsync(ticker).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var marketPrice = new MarketPrices(new MarketPrice(Network, context.Pair, r.lastPrice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.askPrice, r.bidPrice, r.lowPrice, r.highPrice),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });

            return marketPrice;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetSymbolPriceTickerAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var assetPairs = new AssetPairs();

            foreach (var rPrice in r.OrderBy(x => x.symbol.Length))
            {
                var pair = AssetsUtilities.GetAssetPair(rPrice.symbol, assetPairs);

                if (!pair.HasValue)
                    continue;

                assetPairs.Add(new AssetPair(pair.Value.AssetCode1, pair.Value.AssetCode2, this));
            }

            return assetPairs;
        }

        private static readonly int[] OrderBookValidRecordsCount = {5, 10, 20, 50, 100, 200, 500, 1000};

        private void CheckOrderRecordsInputNumber(int? recordsNumber, [CallerMemberName]string method = "Unknown")
        {
            if (recordsNumber.HasValue && !OrderBookValidRecordsCount.Contains(recordsNumber.Value))
                throw new ContextArgumentException("Specified number of order book records is not supported", this, method);
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var maxValidCount = OrderBookValidRecordsCount.Max();
            var recordsCount = context.MaxRecordsCount == Int32.MaxValue ? maxValidCount : context.MaxRecordsCount;

            CheckOrderRecordsInputNumber(recordsCount);

            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var rRaw = await api.GetOrderBookAsync(pairCode, recordsCount).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var orderBook = new OrderBook(Network, context.Pair);

            foreach (var i in r.bids.Select(GetOrderBookRecordData))
                orderBook.AddBid(i.Price, i.Volume, true);
            
            foreach (var i in r.asks.Select(GetOrderBookRecordData))
                orderBook.AddAsk(i.Price, i.Volume, true);

            return orderBook;
        }

        private (decimal Price, decimal Volume) GetOrderBookRecordData(object[] rawEntry)
        {
            if (!decimal.TryParse(rawEntry[0].ToString(), out var price))
                throw new ApiResponseException("Incorrect format of returned price", this);

            if (!decimal.TryParse(rawEntry[1].ToString(), out var volume))
                throw new ApiResponseException("Incorrect format of returned volume", this);

            return (price, volume);
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetAccountInformationAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r != null;
        }
    }
}
