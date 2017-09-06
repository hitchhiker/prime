using System.Threading.Tasks;
using Prime.Core;
using Prime.Utility;
using Rokolab.BitstampClient;
using LiteDB;

namespace plugins
{
    public class BitStampProvider : IExchangeProvider, IWalletService
    {
        public Network Network { get; } = new Network("BitStamp");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly ObjectId IdHash = "prime:bitstamp".GetObjectIdHashCode();

        public ObjectId Id => IdHash;

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return new BitstampClient(context) as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);
            var ra = new RequestAuthenticator(key.Key, key.Secret, key.Extra);
            return new BitstampClient(context, ra) as T;
        }

        public Task<string> TestApi(ApiTestContext context)
        {
            var t = new Task<string>(delegate
            {
                try
                {
                    var api = GetApi<IBitstampClient>(context);
                    var d = api.GetBalance();
                    return d != null && d.Get("status")!="error" ? null : "BAD";
                }
                catch
                {
                    return "BAD";
                }
            });
            return t;
        }

        public ApiConfiguration GetApiConfiguration => new ApiConfiguration()
        {
            HasSecret = true,
            HasExtra = true,
            ApiExtraName = "Customer Number"
        };

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

        private static readonly string _pairs = "btcusd,btceur,eurusd,xrpusd,xrpeur,xrpbtc,ltcusd,ltceur,ltcbtc,ethusd,etheur,ethbtc";

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var t = new Task<AssetPairs>(() => Pairs);
            t.RunSynchronously();
            return t;
        }

        public BalanceResults GetBalance(NetworkProviderPrivateContext context)
        {
            return context.L.Trace("Getting balance from " + Title, () =>
            {

                var api = GetApi<IBitstampClient>(context);

                var market = api.GetBalance();
                var results = new BalanceResults(this);
                foreach (var kvp in market)
                {
                    if (kvp.Key.EndsWith("_available"))
                    {
                        var crc = kvp.Key.Replace("_available", "");
                        results.AddAvailable(crc.ToAsset(this), kvp.Value.ToDecimal());
                    }
                    else if (kvp.Key.EndsWith("_reserved"))
                    {
                        var crc = kvp.Key.Replace("_reserved", "");
                        results.AddReserved(crc.ToAsset(this), kvp.Value.ToDecimal());
                    }
                    else if (kvp.Key.EndsWith("_balance"))
                    {
                        var crc = kvp.Key.Replace("_balance", "");
                        results.AddBalance(crc.ToAsset(this), kvp.Value.ToDecimal());
                    }
                }
                context.L.Trace("Completed balance from " + Title);

                return results;
            });
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool CanMultiDepositAddress => true;

        public bool CanGenerateDepositAddress => false;

        public WalletAddresses FetchAllDepositAddresses(WalletAddressContext context)
        {
            var addresses = new WalletAddresses();

            var wac = new WalletAddressAssetContext("ETH".ToAsset(this), context.CanGenerateAddress, context.UserContext, context.L);
            addresses.AddRange(FetchDepositAddresses(wac));
            wac.Asset = "BTC".ToAsset(this);
            addresses.AddRange(FetchDepositAddresses(wac));
            wac.Asset = "XRP".ToAsset(this);
            addresses.AddRange(FetchDepositAddresses(wac));
            wac.Asset = "LTC".ToAsset(this);
            addresses.AddRange(FetchDepositAddresses(wac));

            return addresses;
        }

        public WalletAddresses FetchDepositAddresses(WalletAddressAssetContext context)
        {
            if (!this.ExchangeHas(context.Asset))
                return null;

            return new WalletAddresses(GetApi<IBitstampClient>(context).GetDepositAddress(this, context.Asset));
        }

        public OhclData GetOhlc(OhlcContext context)
        {
            return null;
        }
    }
}