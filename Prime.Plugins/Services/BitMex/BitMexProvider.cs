using System;
using System.Collections.Generic;
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

        private static readonly string _pairs = "xbtusd";

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);

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

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = GetApi<IBitMexApi>(context);

            var resolution = ConvertToBitMexInterval(context.Market);
            var startDate = context.Range.UtcFrom;
            var endDate = context.Range.UtcTo;

            var r = await api.GetTradeHistory(context.Pair.Asset1.ToRemoteCode(this), resolution, startDate, endDate);

            var ohlc = new OhlcData(context.Market);
            var seriesId = OhlcResolutionAdapter.GetHash(context.Pair, context.Market, Network);

            foreach (var instrActive in r)
            {
                ohlc.Add(new OhlcEntry(seriesId, instrActive.timestamp, this)
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
            if(context.Assets.Count < 1) 
                throw new ArgumentException("The number of target assets should be greater than 0");

            var api = GetApi<IBitMexApi>(context);
            var r = await api.GetLatestPricesAsync();

            if(r == null || r.Count < 1)
                throw new ApiResponseException("No prices data found", this);

            var remote = context.BaseAsset.ToRemoteCode(this);
            var pairCode = (remote + context.Assets.First().ToRemoteCode(this)).ToLower();

            var data = r.FirstOrDefault(x =>
                x.symbol.ToLower().Equals(pairCode)
            );

            if (data == null || data.lastPrice.HasValue == false)
                throw new ApiResponseException("No price returned for selected currency");

            var pricesList = new List<Money>();
            pricesList.Add(new Money(data.lastPrice.Value, data.quoteCurrency.ToAsset(this)));

            // BUG: What UTC Created to set if different currencies have different last price time?
            var latestPrices = new LatestPrices()
            {
                BaseAsset = context.BaseAsset,
                UtcCreated = DateTime.UtcNow,
                Prices = pricesList
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

            // This code fetches all pairs including futures which are not supported for this moment.

            /* var api = GetApi<IBitMexApi>(context);
            var r = await api.GetInstrumentsActive();
            var aps = new AssetPairs();
            foreach (var i in r)
            {
                var ap = new AssetPair(i.underlying.ToAsset(this), i.quoteCurrency.ToAsset(this));
                aps.Add(ap);
            } */
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = GetApi<IBitMexApi>(context);
            var r = await api.GetUserInfoAsync();
            return r != null;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var addresses = new WalletAddresses();

            var api = GetApi<IBitMexApi>(context);

            var depositAddress = await api.GetUserDepositAddressAsync(context.Asset.ToRemoteCode(this));

            var walletAddress = new WalletAddress(this, context.Asset) {Address = depositAddress};

            addresses.Add(walletAddress);

            return addresses;
        }

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
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
