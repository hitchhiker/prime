using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using plugins;
using Prime.Core;
using Prime.Plugins.Services.Poloniex;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class PoloniexTests
        {
            public void GetBalances()
            {
                var provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
                var privateProvider = new NetworkProviderPrivateContext(UserContext.Current);

                try
                {
                    var balances = AsyncContext.Run(() => provider.GetBalancesAsync(privateProvider));

                    foreach (var balance in balances)
                    {
                        Console.WriteLine($"{balance.Asset}: {balance.Balance}, {balance.Available}, {balance.Reserved}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void ApiTest()
            {
                var provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
                var apiTestCtx = new ApiTestContext(UserContext.Current.GetApiKey(provider));

                try
                {
                    var ok = AsyncContext.Run(() => provider.TestApiAsync(apiTestCtx));

                    Console.WriteLine($"Api test OK: {ok}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void AssetsTest()
            {
                var provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
                var ctx = new NetworkProviderContext();

                try
                {
                    var pairs = AsyncContext.Run(() => provider.GetAssetPairs(ctx));

                    foreach (var pair in pairs)
                    {
                        Console.WriteLine($"{pair}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            public void LatestPrices()
            {
                var provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
                var pair = new AssetPair("BTC", "ETH");

                var ctx = new PublicPriceContext(pair);

                try
                {
                    var price = AsyncContext.Run(() => provider.GetLatestPriceAsync(ctx));

                    Console.WriteLine($"Latest price for {pair} is {price.Price}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            public void GetDepositAddresses()
            {
                var provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();

                var ctx = new WalletAddressAssetContext("BTC".ToAsset(provider), false, UserContext.Current);

                try
                {
                    var addresses = AsyncContext.Run(() => provider.GetDepositAddressesAsync(ctx));

                    foreach (var address in addresses)
                    {
                        Console.WriteLine($"{address.Asset} : {address.Address}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
