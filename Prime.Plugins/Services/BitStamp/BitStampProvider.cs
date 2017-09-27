using System.Threading.Tasks;
using Prime.Core;
using Prime.Utility;
using Rokolab.BitstampClient;
using LiteDB;
using Prime.Plugins.Services.Base;
using RestEase;

namespace Prime.Plugins.Services.BitStamp
{
    public class BitStampProvider : IExchangeProvider, IWalletService, IApiProvider
    {
        private const string BitStampApiUrl = "https://www.bitstamp.net/api/v2";

        public Network Network { get; } = new Network("BitStamp");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly ObjectId IdHash = "prime:bitstamp".GetObjectIdHashCode();

        public ObjectId Id => IdHash;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return RestClient.For<IBitStampApi>(BitStampApiUrl) as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);
            var ra = new RequestAuthenticator(key.Key, key.Secret, key.Extra);
            return RestClient.For<IBitStampApi>(BitStampApiUrl, new BitStampAuthenticator(key).GetRequestModifier) as T;
        }

        public Task<bool> TestApiAsync(ApiTestContext context)
        {
            var t = new Task<bool>(() =>
            {
                var api = GetApi<IBitstampClient>(context);
                var r = api.GetBalance();
                return r != null && r.Get("status") != "error";
            });
            t.Start();
            return t;
        }

        public ApiConfiguration GetApiConfiguration => new ApiConfiguration()
        {
            HasSecret = true,
            HasExtra = true,
            ApiExtraName = "Customer Number"
        };

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = GetApi<IBitStampApi>(context);

            var r = await api.GetTicker(context.Pair.TickerSimple());

            var latestPrice = new LatestPrice()
            {
                Price = new Money(r.last, context.Pair.Asset2),
                BaseAsset = context.Pair.Asset1,
                UtcCreated = r.timestamp.ToUtcDateTime()
            };
            
            return latestPrice;
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

        public Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var t = new Task<BalanceResults>(() =>
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

                return results;
            });
            t.Start();
            return t;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool CanMultiDepositAddress => true;

        public bool CanGenerateDepositAddress => false;

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var addresses = new WalletAddresses();
            var wac = new WalletAddressAssetContext("ETH".ToAsset(this), context.CanGenerateAddress, context.UserContext, context.L);
            addresses.AddRange(await GetAddressesForAssetAsync(wac));
            wac.Asset = "BTC".ToAsset(this);
            addresses.AddRange(await GetAddressesForAssetAsync(wac));
            wac.Asset = "XRP".ToAsset(this);
            addresses.AddRange(await GetAddressesForAssetAsync(wac));
            wac.Asset = "LTC".ToAsset(this);
            addresses.AddRange(await GetAddressesForAssetAsync(wac));
            return addresses;
        }

        public Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var t = new Task<WalletAddresses>(() =>
            {
                if (!this.ExchangeHas(context.Asset))
                    return null;

                return new WalletAddresses(GetApi<IBitstampClient>(context).GetDepositAddress(this, context.Asset));
            });

            t.Start();
            return t;
        }
    }
}