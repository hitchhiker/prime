using System;
using System.Collections;
using System.Threading.Tasks;
using Bittrex;
using Prime.Core;
using Prime.Utility;
using Rokolab.BitstampClient;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace plugins
{
    public class BittrexProvider : IExchangeProvider, IWalletService
    {
        public Network Network { get; } = new Network("Bittrex");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly ObjectId IdHash = "prime:bittrex".GetObjectIdHashCode();
            //3d3bdcb685a3455f965f0e78ead0cbba
        public ObjectId Id => IdHash;

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            var client = new Bittrex.Exchange();
            client.Initialise(new ExchangeContext());
            return client as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);
            var client = new Bittrex.Exchange();
            client.Initialise(new ExchangeContext() { ApiKey = key.Key, Secret = key.Secret, QuoteCurrency = "USD" });
            return client as T;
        }
        public Task<string> TestApi(ApiTestContext context)
        {
            var t = new Task<string>(delegate
            {
                try
                {
                    var api = GetApi<Bittrex.Exchange>(context);
                    var d = api.GetBalance("btc");
                    return d != null ? null : "BAD";
                }
                catch
                {
                    return "BAD";
                }
            });
            return t;
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public Task<Money> GetLastPrice(PublicPriceContext context)
        {
            var t=  new Task<Money>(()=> Money.Zero);
            t.RunSynchronously();
            return t;
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

            t.RunSynchronously();
            return t;
        }


        public BalanceResults GetBalance(NetworkProviderPrivateContext context)
        {
            return context.L.Trace("Getting balance from " + Title, () =>
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
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool CanMultiDepositAddress => false;

        public bool CanGenerateDepositAddress => true;

        public WalletAddresses FetchAllDepositAddresses(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public WalletAddresses FetchDepositAddresses(WalletAddressAssetContext context)
        {
            if (!this.ExchangeHas(context.Asset))
                return null;

            return null;
        }

        public OhclData GetOhlc(AssetPair pair, TimeResolution market)
        {
            return null;
        }
    }
}