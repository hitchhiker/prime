using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Bittrex
{
    // https://bittrex.com/home/api
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class BittrexProvider : IBalanceProvider, IOrderBookProvider, IPublicPricingProvider, IAssetPairsProvider, IDepositProvider
    {
        // TODO: AY implement multi-statistics.

        private const string BittrexApiVersion = "v1.1";
        private const string BittrexApiUrl = "https://bittrex.com/api/" + BittrexApiVersion;

        private static readonly IReadOnlyList<Asset> Suspended = "OC,CRYPT,ABY,PIVX,SLING,TROLL,DYN".ToAssetsCsvRaw();

        private static readonly ObjectId IdHash = "prime:bittrex".GetObjectIdHashCode();

        // No information in API documents.
        // https://bitcoin.stackexchange.com/questions/53778/bittrex-api-rate-limit
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(60, 1);

        private RestApiClientProvider<IBittrexApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Bittrex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        private static char _commonPairSep = '-';
        public char? CommonPairSeparator => _commonPairSep;

        public bool IsDirect => true;

        /// <summary>
        /// Only allows new address generating if it is empty. Otherwise only peeking.
        /// </summary>
        public bool CanGenerateDepositAddress => true;

        public bool CanPeekDepositAddress => false;
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BittrexProvider()
        {
            ApiProvider = new RestApiClientProvider<IBittrexApi>(BittrexApiUrl, this, k => new BittrexAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var r = await GetAssetPairsAsync(context).ConfigureAwait(false);

            return r.Count > 0;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAllBalancesAsync().ConfigureAwait(false);

            CheckResponseErrors(r);

            return r != null && r.success && r.result != null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() {CanVolume = true, CanStatistics = true},
            Bulk = new PricingBulkFeatures()
            {
                CanReturnAll = true,
                CanStatistics = true,
                CanVolume = true
            }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetMarketSummaryAsync(pairCode).ConfigureAwait(false);

            CheckResponseErrors(r, context.Pair);

            var e = r.result.FirstOrDefault();
            if (e == null)
                throw new NoAssetPairException(context.Pair, this);

            var price = new MarketPrice(Network, context.Pair.Asset1, new Money(1 / e.Last, context.Pair.Asset2))
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, e.Ask, e.Bid, e.Low, e.High),
                Volume = new NetworkPairVolume(Network, context.Pair, e.BaseVolume, e.Volume)
            };
            return new MarketPrices(price);
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetMarketSummariesAsync().ConfigureAwait(false);

            CheckResponseErrors(r);

            var rPairsDict = r.result.ToDictionary(x => x.MarketName.ToAssetPair(this), x => x);

            var pairsQueryable = context.IsRequestAll
                ? rPairsDict.Keys.ToList()
                : context.Pairs;

            var prices = new MarketPrices();

            foreach (var pair in pairsQueryable)
            {
                if (!rPairsDict.TryGetValue(pair, out var e))
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.Add(new MarketPrice(Network, pair, 1 / e.Last)
                {
                    PriceStatistics = new PriceStatistics(Network, pair.Asset2, e.Ask, e.Bid, e.Low, e.High),
                    Volume = new NetworkPairVolume(Network, pair, e.BaseVolume, e.Volume)
                });
            }

            return prices;
        }

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        public bool DoesMultiplePairs => false;

        public bool PricesAsAssetQuotes => false;

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetMarketsAsync().ConfigureAwait(false);

            CheckResponseErrors(r);

            var pairs = new AssetPairs();

            foreach (var rEntry in r.result)
            {
                var pair = new AssetPair(rEntry.BaseCurrency, rEntry.MarketCurrency);
                pairs.Add(pair);
            }

            return pairs;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAllBalancesAsync().ConfigureAwait(false);
            CheckResponseErrors(r);

            var balances = new BalanceResults();

            foreach (var rBalance in r.result)
            {
                var asset = rBalance.Currency.ToAsset(this);
                balances.Add(asset, rBalance.Available, rBalance.Pending);
            }

            return balances;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context)
        {
            return Task.FromResult(new TransferSuspensions(Suspended, Suspended));
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAllBalancesAsync().ConfigureAwait(false);
            CheckResponseErrors(r);

            var addresses = new WalletAddresses();

            foreach (var rBalance in r.result)
            {
                addresses.Add(new WalletAddress(this, rBalance.Currency.ToAsset(this))
                {
                    Address = rBalance.CryptoAddress
                });
            }

            return addresses;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAllBalancesAsync().ConfigureAwait(false);

            CheckResponseErrors(r);

            var addresses = new WalletAddresses();

            var address = r.result.FirstOrDefault(x => x.Currency.ToAsset(this).Equals(context.Asset));

            if (address != null)
            {
                addresses.Add(new WalletAddress(this, context.Asset)
                {
                    Address = address.CryptoAddress
                });
            }

            return addresses;
        }

        private void CheckResponseErrors<T>(BittrexSchema.BaseResponse<T> response, AssetPair pair = null)
        {
            if (response.success == false)
            {
                if(response.message.Equals("INVALID_MARKET") && pair != null)
                    throw new NoAssetPairException(pair, this);
                throw new ApiResponseException($"API error: {response.message}", this);
            }
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this);

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);

            CheckResponseErrors(r, context.Pair);

            var orderBook = new OrderBook(Network, context.Pair.Reversed); //HH: This is the reversed pair that is returned.

            var bids = context.MaxRecordsCount == Int32.MaxValue
                ? r.result.buy
                : r.result.buy.Take(context.MaxRecordsCount);
            var asks = context.MaxRecordsCount == Int32.MaxValue
                ? r.result.sell
                : r.result.sell.Take(context.MaxRecordsCount);

            foreach (var i in bids)
                orderBook.AddBid(i.Rate, i.Quantity, true); //HH:CONFIRMED INVERTED ON https://bittrex.com/Market/Index?MarketName=BTC-BTCD

            foreach (var i in asks)
                orderBook.AddAsk(i.Rate, i.Quantity, true); 

            return orderBook;
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();
            var r = await api.GetMarketSummaryAsync(pairCode).ConfigureAwait(false);

            var summary = r.result.FirstOrDefault();
            var remoteMarker = summary.MarketName.ToAssetPair(this);
            if (summary == null || !remoteMarker.Equals(context.Pair))
                throw new NoAssetPairException(context.Pair, this);

            return new PublicVolumeResponse(Network, context.Pair, summary.BaseVolume, summary.Volume);
        }

        public VolumeFeatures VolumeFeatures { get; }
    }
}