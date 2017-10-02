using System;
using System.Threading.Tasks;
using Bittrex;
using LiteDB;
using Newtonsoft.Json.Linq;
using Prime.Core;
using Prime.Plugins.Services.Base;
using Prime.Plugins.Services.BitMex;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.Bittrex
{
    public class BittrexProvider : IExchangeProvider, IWalletService, IApiProvider
    {
        private const string BittrexApiVersion = "v1.1";
        private const string BittrexApiUrl = "https://bittrex.com/api/" + BittrexApiVersion;

        public Network Network { get; } = new Network("Bittrex");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly ObjectId IdHash = "prime:bittrex".GetObjectIdHashCode();
        //3d3bdcb685a3455f965f0e78ead0cbba
        public ObjectId Id => IdHash;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return RestClient.For<IBittrexApi>(BittrexApiUrl) as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);
            return RestClient.For<IBittrexApi>(BittrexApiUrl, new BittrexAuthenticator(key).GetRequestModifier) as T;
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = GetApi<IBittrexApi>(context);
            var r = await api.GetAllBalances();

            return r != null && r.success && r.result != null;
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            return null;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            return null;
        }

        public SellResult Sell(SellContext ctx)
        {
            return null;
        }

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var t = new Task<AssetPairs>(() =>
            {
                var api = this.GetApi<Exchange>(context);
                var da = api.GetMarkets();
                var aps = new AssetPairs();
                var results = (JArray)da;

                foreach (var mr in results)
                {
                    var m = mr["MarketCurrency"].ToString();
                    var bc = mr["BaseCurrency"].ToString();
                    var pair = new AssetPair(m.ToAsset(this), bc.ToAsset(this));
                    aps.Add(pair);
                }
                return aps;
            });

            t.Start();
            return t;
        }


        public Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var t = new Task<BalanceResults>(() =>
            {
                var api = GetApi<Exchange>(context);
                var market = api.GetBalances();
                var results = new BalanceResults(this);
                foreach (var kvp in market)
                {
                    var asset = kvp.Currency.ToAsset(this);
                    results.AddBalance(asset, kvp.Balance);
                    results.AddAvailable(asset, kvp.Available);
                    results.AddReserved(asset, kvp.Pending);
                }
                return results;
            });
            t.Start();
            return t;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool CanMultiDepositAddress => false;

        public bool CanGenerateDepositAddress => true;

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            if (!this.ExchangeHas(context.Asset))
                return null;

            return null;
        }

        public OhlcData GetOhlc(AssetPair pair, TimeResolution market)
        {
            return null;
        }
    }
}