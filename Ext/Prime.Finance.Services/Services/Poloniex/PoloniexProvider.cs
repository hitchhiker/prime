using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.Poloniex
{
    // https://poloniex.com/support/api/
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class PoloniexProvider : IBalanceProvider, IOhlcProvider, IOrderBookProvider, IDepositProvider, IPublicPricingProvider, IAssetPairsProvider
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

        private static readonly IRateLimiter Limiter = new PerSecondRateLimiter(6, 1);
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => true;

        public PoloniexProvider()
        {
            ApiProvider = new RestApiClientProvider<IPoloniexApi>(PoloniexApiUrl, this, k => new PoloniexAuthenticator(k).GetRequestModifierAsync);
        }

        private void CheckResponseErrors<T>(Response<T> response, [CallerMemberName] string method = "Unknown")
        {
            if (response.TryGetContent<T, PoloniexSchema.ErrorResponse>(out var rError))
                if (!string.IsNullOrWhiteSpace(rError.error))
                    throw new ApiResponseException(rError.error.Trim(new char[] { '.' }), this, method);

            if (!response.ResponseMessage.IsSuccessStatusCode)
                throw new ApiResponseException($"API error occurred: {response.ResponseMessage.ReasonPhrase} ({response.ResponseMessage.StatusCode})", this, method);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var r = await GetAssetPairsAsync(context).ConfigureAwait(false);

            return r.Count > 0;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreatePoloniexBody(PoloniexBodyType.ReturnBalances);

            var rRaw = await api.GetBalancesAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r != null && r.Count > 0;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, '_');

            var rRaw = context.MaxRecordsCount == Int32.MaxValue
                ? await api.GetOrderBookAsync(pairCode).ConfigureAwait(false)
                : await api.GetOrderBookAsync(pairCode, context.MaxRecordsCount).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            if (r.bids == null || r.asks == null)
                throw new AssetPairNotSupportedException(context.Pair, this);

            var orderBook = new OrderBook(Network, context.Pair.Reversed); //POLONIEX IS REVERSING THE MARKET

            foreach (var i in r.bids)
                orderBook.AddBid(i[0], i[1], true); //HH: Confirmed reversed on https://poloniex.com/exchange#btc_btcd

            foreach (var i in r.asks)
                orderBook.AddAsk(i[0], i[1], true);

            return orderBook.AsPair(context.Pair);
        }

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetTickerAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var rPaired = r.ToDictionary(x => x.Key.ToAssetPair(this), y => y.Value);
            var pairsQueryable = context.IsRequestAll ? rPaired.Select(x => x.Key) : context.Pairs;

            var prices = new MarketPrices();

            foreach (var pair in pairsQueryable)
            {
                var rTickers = rPaired.Where(x => x.Key.Equals(pair)).ToList();

                if (rTickers.Count == 0)
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                var rTicker = rTickers[0];
                var v = rTicker.Value;

                var price = new MarketPrice(Network, pair.Reversed, v.last)
                {
                    PriceStatistics = new PriceStatistics(Network, pair.Asset2, v.lowestAsk, v.highestBid, v.low24hr, v.high24hr),
                    Volume = new NetworkPairVolume(Network, pair, v.baseVolume, v.quoteVolume)
                };

                prices.Add(price.Reversed);
            }

            return prices;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetTickerAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var pairs = new AssetPairs();

            foreach (var rPair in r)
            {
                var pair = rPair.Key.ToAssetPair(this);

                pairs.Add(pair);
            }

            return pairs;
        }

        public Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context) =>
            Task.FromResult<TransferSuspensions>(null);

        public async Task<WalletAddressesResult> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreatePoloniexBody(PoloniexBodyType.ReturnDepositAddresses);

            var addresses = new WalletAddressesResult();

            var rRaw = await api.GetDepositAddressesAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            foreach (var balance in r)
            {
                if (!string.IsNullOrWhiteSpace(balance.Value))
                    addresses.Add(new WalletAddress(this, balance.Key.ToAsset(this)) { Address = balance.Value });
            }

            return addresses;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreatePoloniexBody(PoloniexBodyType.ReturnCompleteBalances);

            var rRaw = await api.GetBalancesDetailedAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var results = new BalanceResults(this);

            foreach (var kvp in r)
            {
                var c = kvp.Key.ToAsset(this);
                results.Add(c, kvp.Value.available, kvp.Value.onOrders);
            }

            return results;
        }

        private Dictionary<string, object> CreatePoloniexBody(PoloniexBodyType bodyType)
        {
            var body = new Dictionary<string, object> { { "nonce", BaseAuthenticator.GetLongNonce() } };

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
                case PoloniexBodyType.LimitOrderBuy:
                    body.Add("command", "buy");
                    break;
                case PoloniexBodyType.LimitOrderSell:
                    body.Add("command", "sell");
                    break;
                case PoloniexBodyType.ReturnOrderStatus:
                    body.Add("command", "returnOrderTrades");
                    break;
                case PoloniexBodyType.ReturnOpenOrders:
                    body.Add("command", "returnOpenOrders");
                    break;
                case PoloniexBodyType.ReturnTradeHistory:
                    body.Add("command", "returnTradeHistory");
                    break;
                case PoloniexBodyType.CancelOrder:
                    body.Add("command", "cancelOrder");
                    break;
                case PoloniexBodyType.Withdraw:
                    body.Add("command", "withdraw");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null);
            }

            return body;
        }

        public IAssetCodeConverter GetAssetCodeConverter() => null;

        public async Task<WalletAddressesResult> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreatePoloniexBody(PoloniexBodyType.ReturnDepositAddresses);

            var addresses = new WalletAddressesResult();

            var rRaw = await api.GetDepositAddressesAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var assetBalances = r.Where(x => Equals(x.Key.ToAsset(this), context.Asset)).ToArray();

            foreach (var balance in assetBalances)
            {
                if (string.IsNullOrWhiteSpace(balance.Value))
                    continue;

                addresses.Add(new WalletAddress(this, balance.Key.ToAsset(this)) { Address = balance.Value });
            }

            return addresses;
        }

        public async Task<OhlcDataResponse> GetOhlcAsync(OhlcContext context)
        {
            var pair = context.Pair;
            var resolution = context.Resolution;

            var timeStampStart = (long)context.Range.UtcFrom.ToUnixTimeStamp();
            var timeStampEnd = (long)context.Range.UtcTo.ToUnixTimeStamp();

            var period = ConvertToPoloniexInterval(resolution);

            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetChartDataAsync(pair.ToTicker(this), timeStampStart, timeStampEnd, period).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var ohlc = new OhlcDataResponse(resolution);
            var seriesid = OhlcUtilities.GetHash(pair, resolution, Network);

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
            // TODO: AY: implement all TimeResolution cases.
            switch (resolution)
            {
                case TimeResolution.Day:
                    return PoloniexTimeInterval.Day1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.Get24HVolumeAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var volumes = r.Where(x => x.Key.ToAssetPair(this).Equals(context.Pair)).ToList();

            if (!volumes.Any())
                throw new AssetPairNotSupportedException(context.Pair, this);

            var rVolumes = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(volumes.FirstOrDefault().Value.ToString());
            if (!rVolumes.TryGetValue(context.Pair.Asset1.ShortCode, out var volume))
                throw new AssetPairNotSupportedException(context.Pair, this);

            return new PublicVolumeResponse(Network, context.Pair, volume);
        }
    }
}
