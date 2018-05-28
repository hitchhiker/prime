using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Services.Services.BitMex
{
    // https://www.bitmex.com/api/explorer/
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class BitMexProvider : IOhlcProvider, IOrderBookProvider, IPublicPricingProvider, IAssetPairsProvider
    {
        public Version Version { get; } = new Version(1, 0, 0);
        private static readonly ObjectId IdHash = "prime:bitmex".GetObjectIdHashCode();

        private const String BitMexApiUrl = "https://www.bitmex.com/api/v1";
        private const String BitMexApiUrlTest = "https://testnet.bitmex.com/api/v1";

        private static readonly string _pairs = "btcusd";
        private const decimal ConversionRate = 0.00000001m;

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);

        private RestApiClientProvider<IBitMexApi> ApiProvider { get; }
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(150, 5, 300, 5);

        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;
        public ObjectId Id => IdHash;
        public Network Network => Networks.I.Get("BitMex");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        public BitMexProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitMexApi>(BitMexApiUrl, this, (k) => new BitMexAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetConnectedUsersAsync().ConfigureAwait(false);

            return r != null;
        }

        private string ConvertToBitMexInterval(TimeResolution market)
        {
            switch (market)
            {
                case TimeResolution.Minute:
                    return "1m";
                case TimeResolution.Hour:
                    return "1h";
                case TimeResolution.Day:
                    return "1d";
                default:
                    throw new ArgumentOutOfRangeException(nameof(market), market, null);
            }
        }

        public async Task<OhlcDataResponse> GetOhlcAsync(OhlcContext context)
        {
            var api = ApiProvider.GetApi(context);

            var resolution = ConvertToBitMexInterval(context.Resolution);
            var startDate = context.Range.UtcFrom;
            var endDate = context.Range.UtcTo;

            var rRaw = await api.GetTradeHistoryAsync(context.Pair.Asset1.ToRemoteCode(this), resolution, startDate, endDate).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var ohlc = new OhlcDataResponse(context.Resolution);
            var seriesId = OhlcUtilities.GetHash(context.Pair, context.Resolution, Network);

            foreach (var instrActive in r)
            {
                ohlc.Add(new OhlcEntry(seriesId, instrActive.timestamp, this)
                {
                    Open = instrActive.open,
                    Close = instrActive.close,
                    Low = instrActive.low,
                    High = instrActive.high,
                    VolumeTo = instrActive.volume,
                    VolumeFrom = instrActive.volume,
                    WeightedAverage = (instrActive.vwap ?? 0) // BUG: what to set if vwap is NULL?
                });
            }

            return ohlc;
        }

        private static readonly IAssetCodeConverter AssetCodeConverter = new BitMexCodeConverter();
        public IAssetCodeConverter GetAssetCodeConverter() => AssetCodeConverter;

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true, SupportsMultipleQuotes = false }
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
            var rRaw = await api.GetLatestPricesAsync(context.Pair.Asset1.ToRemoteCode(this)).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var rPrice = r.FirstOrDefault(x => x.symbol.Equals(context.Pair.ToTicker(this, "")));

            if (rPrice == null || rPrice.lastPrice.HasValue == false)
                throw new AssetPairNotSupportedException(context.Pair, this);

            var price = new MarketPrice(Network, context.Pair, rPrice.lastPrice.Value)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, rPrice.askPrice, rPrice.bidPrice, rPrice.lowPrice, rPrice.highPrice),
                Volume = new NetworkPairVolume(Network, context.Pair, rPrice.volume24h)
            };

            return new MarketPrices(price);
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetLatestPricesAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var pairsDict = r.ToDictionary(x => new AssetPair(x.underlying, x.quoteCurrency, this), x => x);

            var pairsQueryable = context.IsRequestAll
                ? pairsDict.Keys.ToList()
                : context.Pairs;

            var prices = new MarketPrices();

            foreach (var pair in pairsQueryable)
            {
                if (!pairsDict.TryGetValue(pair, out var data) || data.lastPrice.HasValue == false)
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.Add(new MarketPrice(Network, pair, data.lastPrice.Value)
                {
                    PriceStatistics = new PriceStatistics(Network, pair.Asset2, data.askPrice, data.bidPrice, data.lowPrice, data.highPrice),
                    Volume = new NetworkPairVolume(Network, pair, data.volume24h)
                });
            }

            return prices;
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context) => Task.Run(() => Pairs);

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetUserInfoAsync().ConfigureAwait(false);
            return r != null;
        }

        public Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context) => Task.FromResult<TransferSuspensions>(null);

        public Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this, "");

            var rRaw = context.MaxRecordsCount == Int32.MaxValue
                ? await api.GetOrderBookAsync(pairCode, 0).ConfigureAwait(false)
                : await api.GetOrderBookAsync(pairCode, context.MaxRecordsCount).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var buyAction = "buy";
            var sellAction = "sell";

            var buys = context.MaxRecordsCount == Int32.MaxValue
                ? r.Where(x => x.side.ToLower().Equals(buyAction)).OrderBy(x => x.id).ToList()
                : r.Where(x => x.side.Equals(buyAction, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.id)
                    .Take(context.MaxRecordsCount).ToList();

            var sells = context.MaxRecordsCount == Int32.MaxValue
                ? r.Where(x => x.side.ToLower().Equals(sellAction)).OrderBy(x => x.id).ToList()
                : r.Where(x => x.side.Equals(sellAction, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.id)
                    .Take(context.MaxRecordsCount).ToList();

            var orderBook = new OrderBook(Network, context.Pair);

            foreach (var i in buys)
                orderBook.AddBid(i.price, i.size);

            foreach (var i in sells)
                orderBook.AddAsk(i.price, i.size);

            return orderBook;
        }

        public bool IsWithdrawalFeeIncluded => false;
    }
}

