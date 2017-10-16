using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;
using Prime.Plugins.Services.Base;
using Prime.Utility;

namespace Prime.Plugins.Services.Korbit
{
    public class KorbitProvider : IExchangeProvider, IOrderBookProvider, IWalletService
    {
        private static readonly ObjectId IdHash = "prime:korbit".GetObjectIdHashCode();
        private static readonly string _pairs = "btckrw,etckrw,ethkrw,xrpkrw";
        
        public const string KorbitApiVersion = "v1/";
        public const string KorbitApiUrl = "https://api.korbit.co.kr/" + KorbitApiVersion;

        private RestApiClientProvider<IKorbitApi> ApiProvider { get; }

        public KorbitProvider()
        {
            ApiProvider = new RestApiClientProvider<IKorbitApi>(KorbitApiUrl, this, k => new KorbitAuthenticator(k).GetRequestModifier);
        }

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);
        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("Korbit");
        public bool Disabled { get; } = false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public IRateLimiter RateLimiter { get; } = new NoRateLimits();
        public ApiConfiguration GetApiConfiguration { get; } = ApiConfiguration.Standard2;

        public bool CanMultiDepositAddress { get; } = false;
        public bool CanGenerateDepositAddress { get; } = false;

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = GetKorbitTicker(context.Pair);

            var r = await api.GetTicker(pairCode);

            var sTimeStamp = r.timestamp / 1000; // r.timestamp is returned in ms.

            var latestPrice = new LatestPrice()
            {
                BaseAsset = context.Pair.Asset1,
                UtcCreated = sTimeStamp.ToUtcDateTime(),
                Price = new Money(r.last, context.Pair.Asset2)
            };

            return latestPrice;
        }

        public Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context)
        {
            throw new NotImplementedException();
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
        }

        public Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            throw new NotImplementedException();
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TestApiAsync(ApiTestContext context)
        {
            throw new NotImplementedException();
        }

        public Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            throw new NotImplementedException();
        }

        private string GetKorbitTicker(AssetPair pair)
        {
            return pair.TickerUnderslash().ToLower();
        }
    }
}
