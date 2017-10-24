using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.HitBtc
{
    public class HitBtcProvider : IExchangeProvider, IWalletService, IOhlcProvider, IOrderBookProvider
    {
        private const string HitBtcApiUrl = "https://api.hitbtc.com/api";

        private static readonly ObjectId IdHash = "prime:hitbtc".GetObjectIdHashCode();
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private readonly RestApiClientProvider<IHitBtcApi> ApiProvider;

        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("HitBTC");
        public bool Disabled { get; } = false;
        public int Priority { get; } = 100;
        public string AggregatorName { get; } = null;
        public string Title => Network.Name;
        public IRateLimiter RateLimiter => Limiter;

        public Task<LatestPrice> GetPairPriceAsync(PublicPairPriceContext context)
        {
            throw new NotImplementedException();
        }

        public Task<LatestPrices> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            throw new NotImplementedException();
        }

        public bool CanMultiDepositAddress { get; } = false;
        public bool CanGenerateDepositAddress { get; } = true;
        public ApiConfiguration GetApiConfiguration { get; } = ApiConfiguration.Standard2;

        public HitBtcProvider()
        {
            ApiProvider = new RestApiClientProvider<IHitBtcApi>(HitBtcApiUrl, this, k => new HitBtcAuthenticator(k).GetRequestModifier);
        }

        public Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            throw new NotImplementedException();
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
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
            var api = ApiProvider.GetApi(context);
            var r = await api.GetSymbols();

            var assetPairs = new AssetPairs();

            foreach (var symbol in r.symbols)
            {
                assetPairs.Add(new AssetPair(symbol.currency.ToAsset(this), symbol.commodity.ToAsset(this)));
            }

            return assetPairs;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetDepositAddress(context.Asset.ShortCode);

            var walletAddresses = new WalletAddresses();
            walletAddresses.Add(new WalletAddress(this, context.Asset)
            {
                Address = r.address
            });

            return walletAddresses;
        }

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalances();

            return r != null && r.balance.Any();
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalances();

            var balances = new BalanceResults(this);

            foreach (var rBalance in r.balance)
            {
                balances.Add(new BalanceResult(rBalance.currency_code.ToAsset(this))
                {
                    Available = rBalance.balance,
                    Balance = rBalance.balance,
                    Reserved = 0
                });
            }

            return balances;
        }

        public Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            throw new NotImplementedException();
        }
    }
}
