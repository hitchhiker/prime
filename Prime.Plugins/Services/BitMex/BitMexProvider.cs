using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;
using Prime.Plugins.Services.Base;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.BitMex
{
    public class BitMexProvider : IExchangeProvider, IWalletService, IOhlcProvider, IApiProvider
    {
        private static readonly ObjectId IdHash = "prime:bitmex".GetObjectIdHashCode();

        private const String BitMaxApiUrl = "https://www.bitmex.com/api/v1";

        public BitMexProvider()
        {
            Network = new Network("BitMex");
            Disabled = false;
            AggregatorName = null;
            Title = Network.Name;
            Id = IdHash;
            Priority = 100;
            GetApiConfiguration = ApiConfiguration.Standard2;

            CanGenerateDepositAddress = false;
            CanMultiDepositAddress = false;
        }

        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(150, 5, 300, 5);
        public IRateLimiter RateLimiter => Limiter;

        public async Task<OhclData> GetOhlcAsync(OhlcContext context)
        {
            var api = GetApi<IBitMexApi>(context);

            string resolution = "";

            switch (context.Market)
            {
                case TimeResolution.Minute:
                    resolution = "1m";
                    break;
                case TimeResolution.Hour:
                    resolution = "1h";
                    break;
                case TimeResolution.Day:
                    resolution = "1d";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("BitMex does not support specified time resolution");
            }

            // BUG: how to properly select number of records to receive? Hardcoded default is 100.
            var r = await api.GetTradeHistory(context.Pair.Asset1.ToRemoteCode(this), resolution, 100);

            var ohlc = new OhclData(context.Market);
            var seriesId = OhlcResolutionAdapter.GetHash(context.Pair, context.Market, Network);

            foreach (var instrActive in r)
            {
                ohlc.Add(new OhclEntry(seriesId, instrActive.timestamp, this)
                {
                    Open = (double)instrActive.open,
                    Close = (double)instrActive.close,
                    Low = (double)instrActive.low,
                    High = (double)instrActive.high,
                    VolumeTo = (long)instrActive.volume,
                    VolumeFrom = (long)instrActive.volume,
                    WeightedAverage = (double)instrActive.vwap // Should be checked.
                });
            }

            return ohlc;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return new BitMexCodeConverter();
        }

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = GetApi<IBitMexApi>(context);
            var r = (await api.GetLatestPriceAsync(context.Pair.Asset1.ToRemoteCode(this))).FirstOrDefault();

            if (r == null)
                throw new ApiResponseException("No price data found", this);

            if (r.timestamp.Kind != DateTimeKind.Utc)
                throw new ApiResponseException("Time is not in UTC format", this);

            // TODO: Check this. How to handle NULL in last price value?
            if (r.lastPrice.HasValue == false)
                throw new ApiResponseException("No last price for currency", this);

            var latestPrice = new LatestPrice
            {
                BaseAsset = context.Pair.Asset1,
                Price = new Money(r.lastPrice.Value, context.Pair.Asset2),
                UtcCreated = r.timestamp
            };

            return latestPrice;
        }

        public async Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context)
        {
            var api = GetApi<IBitMexApi>(context);
            var r = await api.GetLatestPricesAsync();

            if(r == null || r.Count < 1)
                throw new ApiResponseException("No prices data found", this);

            // TODO: Will filter currencies and select only those which are not NULL.
            // BUG: There are a lot of pairs (e.g. XBT->ETH) with different symbol names, how to collapse them? User wants to see only XBT->ETH...
            var selectedData = r.Where(x => x.lastPrice.HasValue && x.quoteCurrency.ToAsset(this).Equals(context.BaseAsset) &&
                                             context.Assets.Contains(x.underlying.ToAsset(this)))
                                             .OrderByDescending(x => x.timestamp)
                                             .ToList();

            // BUG: Filtered by combination of quote + underlying.
            var filteredData = selectedData.DistinctBy(x => x.quoteCurrency + x.underlying).ToList();

#if FALSE
            // TODO: Remove from production.

            Console.WriteLine("Selected data:");
            foreach (var response in selectedData)
            {
                Console.WriteLine($"{response.timestamp}: {response.underlying} -> {response.quoteCurrency}, Symbol: {response.symbol}, Last Price: {response.lastPrice}");
            }

            Console.WriteLine("Filtered data:");
            foreach (var response in filteredData)
            {
                Console.WriteLine($"{response.timestamp}: {response.underlying} -> {response.quoteCurrency}, Symbol: {response.symbol}, Last Price: {response.lastPrice}");
            }
#endif

            var latestMoneyList = filteredData.Select(x => new Money(x.lastPrice.Value, x.underlying.ToAsset(this))).ToList();

            // BUG: What UTC Created to set if different currencies have different last price time?
            var latestPrices = new LatestPrices()
            {
                BaseAsset = context.BaseAsset,
                UtcCreated = DateTime.UtcNow,
                Prices = latestMoneyList
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
            var api = GetApi<IBitMexApi>(context);
            var r = await api.GetInstrumentsActive();
            var aps = new AssetPairs();
            foreach (var i in r)
            {
                var ap = new AssetPair(i.underlying.ToAsset(this), i.quoteCurrency.ToAsset(this));
                aps.Add(ap);
            }
            return aps;
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = GetApi<IBitMexApi>(context);
            var r = await api.GetUserInfoAsync();
            return r != null;
        }

        public async Task<WalletAddresses> GetDepositAddressesAsync(WalletAddressAssetContext context)
        {
            var addresses = new WalletAddresses();

            var api = GetApi<IBitMexApi>(context);

            var depositAddress = await  api.GetUserDepositAddressAsync(context.Asset.ToRemoteCode(this));

            var walletAddress = new WalletAddress(this, context.Asset) {Address = depositAddress};

            addresses.Add(walletAddress);

            return addresses;
        }

        public Task<WalletAddresses> FetchAllDepositAddressesAsync(WalletAddressContext context)
        {
            return null;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = GetApi<IBitMexApi>(context);
            var r = await api.GetUserWalletInfoAsync("XBt");

            var results = new BalanceResults(this);

            var btcAmount = (decimal)0.00000001 * r.amount;

            var c = r.currency.ToAsset(this);
            results.AddBalance(c, btcAmount);
            results.AddAvailable(c, btcAmount);
            results.AddReserved(c, 0);

            return results;
        }

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return RestClient.For<IBitMexApi>(BitMaxApiUrl) as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);

            return RestClient.For<IBitMexApi>(BitMaxApiUrl, new BitMexAuthenticator(key).GetRequestModifier) as T;
        }

        public ApiConfiguration GetApiConfiguration { get; }

        public bool CanMultiDepositAddress { get; }

        public bool CanGenerateDepositAddress { get; }

        public ObjectId Id { get; }

        public Network Network { get; }

        public bool Disabled { get; }

        public int Priority { get; }

        public string AggregatorName { get; }

        public string Title { get; }

    }
}
