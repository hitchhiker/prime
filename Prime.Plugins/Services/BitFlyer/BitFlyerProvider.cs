using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BitFlyer
{
    public class BitFlyerProvider : IWalletService, IOrderBookProvider, IExchangeProvider
    {
        public const string BitFlyerApiUrl = "https://api.bitflyer.com/" + BitFlyerApiVersion;
        public const string BitFlyerApiVersion = "v1";

        private static readonly ObjectId IdHash = "prime:bitflyer".GetObjectIdHashCode();

        private RestApiClientProvider<IBitFlyerApi> ApiProvider { get; }

        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("BitFlyer");
        public bool Disabled { get; } = false;
        public int Priority => 100;
        public string AggregatorName { get; }
        public string Title => Network.Name;

        // Each IP address is limited to approx. 500 queries per minute.
        // https://lightning.bitflyer.jp/docs?lang=en#api-limits
        public IRateLimiter RateLimiter { get; } = new PerMinuteRateLimiter(400, 1);

        public ApiConfiguration GetApiConfiguration { get; } = ApiConfiguration.Standard2;

        public bool CanMultiDepositAddress { get; } = false;
        public bool CanGenerateDepositAddress { get; } = false;

        public BitFlyerProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitFlyerApi>(BitFlyerApiUrl, this, k => new BitFlyerAuthenticator(k).GetRequestModifier);
        }

        public Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            throw new NotImplementedException();
        }

        public Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context)
        {
            throw new NotImplementedException();
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            throw new NotImplementedException();
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

        public Task<OrderBook> GetOrderBook(OrderBookContext context)
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

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var assetPairs = new AssetPairs();

            var r = await api.GetPrices();

            foreach (var rPrice in r)
            {
                if (rPrice.product_code.Split('_').Length > 2)
                    continue;

                assetPairs.Add(new AssetPair(rPrice.main_currency, rPrice.sub_currency));
            }

            return assetPairs;
        }
    }
}
