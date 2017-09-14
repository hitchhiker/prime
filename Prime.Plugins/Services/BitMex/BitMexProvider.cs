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

        public OhclData GetOhlc(OhlcContext context)
        {
            throw new NotImplementedException();
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public Task<Money> GetLastPrice(PublicPriceContext context)
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
            throw new NotImplementedException();
        }

        public Task<string> TestApi(ApiTestContext context)
        {
            var task = new Task<string>(delegate
            {
                try
                {
                    IBitMexApi api = GetApi<IBitMexApi>(context);
                    var r = AsyncContext.Run(api.GetUserInfo);

                    return r != null ? null : "BAD";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            });

            return task;
        }

        public WalletAddresses FetchDepositAddresses(WalletAddressAssetContext context)
        {
            var addresses = new WalletAddresses();

            IBitMexApi api = GetApi<IBitMexApi>(context);

            // BUG: Where to store curremcy names?
            String depositAddress = AsyncContext.Run(() => api.GetUserDepositAddress("XBt"));

            WalletAddress walletAddress = new WalletAddress(this, context.Asset);
            walletAddress.Address = depositAddress;

            addresses.Add(walletAddress);

            return addresses;
        }

        public WalletAddresses FetchAllDepositAddresses(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public BalanceResults GetBalance(NetworkProviderPrivateContext context)
        {
            return context.L.Trace($"Getting balance from {Title}", () =>
            {
                IBitMexApi api = GetApi<IBitMexApi>(context);

                // BUG: Where to store curremcy names?
                var r = AsyncContext.Run(() => api.GetUserWalletInfo("XBt"));
                
                var results = new BalanceResults(this);

                var c = r.currency.ToAsset(this);
                results.AddBalance(c, r.amount);
                results.AddAvailable(c, r.amount);
                results.AddReserved(c, r.amount);

                return results;
            });
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
