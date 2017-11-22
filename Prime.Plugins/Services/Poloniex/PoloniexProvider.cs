using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Plugins.Services.Poloniex
{
    // https://poloniex.com/support/api/
    public class PoloniexProvider : IBalanceProvider, IOhlcProvider, IOrderBookProvider, IDepositProvider, IPublicPricingProvider, IAssetPairsProvider, IPublicVolumeProvider
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
        public string CommonPairSeparator { get; }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => true;

        public PoloniexProvider()
        {
            ApiProvider = new RestApiClientProvider<IPoloniexApi>(PoloniexApiUrl, this, k => new PoloniexAuthenticator(k).GetRequestModifier);
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

            try
            {
                var r = await api.GetBalancesAsync(body).ConfigureAwait(false);

                return r != null && r.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Bulk = new PricingBulkFeatures() {CanStatistics = true, CanVolume = true, CanReturnAll = true}
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync().ConfigureAwait(false);

            var rPaired = r.ToDictionary(x => x.Key.ToAssetPair(this, '_'), y => y.Value);
            var pairsQueryable = context.IsRequestAll ? rPaired.Select(x => x.Key) : context.Pairs;
                
            var prices = new MarketPricesResult();

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

                prices.MarketPrices.Add(new MarketPrice(Network, pair, 1/v.last)
                {
                    PriceStatistics = new PriceStatistics(Network, pair.Asset2, v.lowestAsk, v.highestBid, v.low24hr, v.high24hr),
                    Volume = new NetworkPairVolume(Network, pair, v.baseVolume, v.quoteVolume)
                });
            }

            return prices;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickerAsync().ConfigureAwait(false);

            var pairs = new AssetPairs();

            foreach (var rPair in r)
            {
                var pair = rPair.Key.ToAssetPair(this, '_');

                pairs.Add(pair);
            }

            return pairs;
        }

        public Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context)
        {
            return Task.FromResult<TransferSuspensions>(null);
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreatePoloniexBody(PoloniexBodyType.ReturnDepositAddresses);

            var addresses = new WalletAddresses();

            try
            {
                var r = await api.GetDepositAddressesAsync(body).ConfigureAwait(false);

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
        
        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreatePoloniexBody(PoloniexBodyType.ReturnCompleteBalances);

            var r = await api.GetBalancesDetailedAsync(body).ConfigureAwait(false);

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
                var r = await api.GetDepositAddressesAsync(body).ConfigureAwait(false);
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
            var r = await api.GetChartDataAsync(pair.ToTicker(this, "_"), timeStampStart, timeStampEnd, period).ConfigureAwait(false);

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

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, "_");

            var r = context.MaxRecordsCount.HasValue ? await api.GetOrderBookAsync(pairCode, context.MaxRecordsCount.Value / 2).ConfigureAwait(false) : await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);

            if (r.bids == null || r.asks == null)
                throw new NoAssetPairException(context.Pair, this);

            var orderBook = new OrderBook(Network, context.Pair);

            foreach (var i in r.bids)
                orderBook.Add(new OrderBookRecord(OrderType.Bid, new Money(i[0], context.Pair.Asset2), i[1]));

            foreach (var i in r.asks)
                orderBook.Add(new OrderBookRecord(OrderType.Ask, new Money(i[0], context.Pair.Asset2), i[1]));

            return orderBook;
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.Get24HVolumeAsync().ConfigureAwait(false);
            var volumes = r.Where(x => x.Key.ToAssetPair(this, '_').Equals(context.Pair));

            if (!volumes.Any())
                throw new NoAssetPairException(context.Pair, this);

            var rVolumes = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(volumes.FirstOrDefault().Value.ToString());
            if(!rVolumes.TryGetValue(context.Pair.Asset1.ShortCode, out var volume))
                throw new NoAssetPairException(context.Pair, this);

            return new PublicVolumeResponse(Network, context.Pair, volume);
        }

        public VolumeFeatures VolumeFeatures { get; }
    }
}
