using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Nito.AsyncEx;
using Prime.Core;
using Prime.Plugins.Services.Base;
using Prime.Plugins.Services.BitMex;
using Prime.Utility;
using RestEase;

namespace plugins
{
    public class BitMexProvider : IExchangeProvider, IWalletService, IOhlcProvider, IApiProvider
    {
        private static readonly ObjectId IdHash = "prime:bitmex".GetObjectIdHashCode();

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

        public Task<OhclData> GetOhlcAsync(OhlcContext context)
        {
            throw new NotImplementedException();
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return new BitMexCodeConverter();
        }

        public Task<LatestPrices> GetLatestPricesAsync(Asset asset, List<Asset> assets)
        {
            throw new NotImplementedException();
        }

        public Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
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
            var r = await api.GetUserInfo();
            return r != null;
        }

        public async Task<WalletAddresses> GetDepositAddressesAsync(WalletAddressAssetContext context)
        {
            var addresses = new WalletAddresses();

            var api = GetApi<IBitMexApi>(context);

            var depositAddress = await  api.GetUserDepositAddress(context.Asset.ToRemoteCode(this));

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
            var r = await api.GetUserWalletInfo("XBt");
                
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
            return RestClient.For<IBitMexApi>("https://www.bitmex.com") as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);
            return RestClient.For<IBitMexApi>("https://www.bitmex.com", new BitMexAuthenticator(key).GetRequestModifier) as T;
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
