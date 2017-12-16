using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using plugins;
using Prime.Common;
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
                        System.Console.WriteLine($"{balance.Asset}: {balance.AvailableAndReserved}, {balance.Available}, {balance.Reserved}");
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void ApiTest()
            {
                var provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
                var apiTestCtx = new ApiPrivateTestContext(UserContext.Current.GetApiKey(provider));

                try
                {
                    var ok = AsyncContext.Run(() => provider.TestPrivateApiAsync(apiTestCtx));

                    System.Console.WriteLine($"Api test OK: {ok}");
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void AssetsTest()
            {
                var provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
                var ctx = new NetworkProviderContext();

                try
                {
                    var pairs = AsyncContext.Run(() => provider.GetAssetPairsAsync(ctx));

                    foreach (var pair in pairs)
                    {
                        System.Console.WriteLine($"{pair}");
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                    throw;
                }
            }

            public void GetDepositAddresses()
            {
                var provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();

                var ctx = new WalletAddressAssetContext("BTC".ToAsset(provider), UserContext.Current);
                var ctxAll = new WalletAddressContext(UserContext.Current);

                try
                {
                    var addresses = AsyncContext.Run(() => provider.GetAddressesForAssetAsync(ctx));
                    var addressesAll = AsyncContext.Run(() => provider.GetAddressesAsync(ctxAll));

                    System.Console.WriteLine("Addresses for 1 asset");
                    foreach (var address in addresses)
                    {
                        System.Console.WriteLine($"{address.Asset} : {address.Address}");
                    }

                    System.Console.WriteLine("Addresses for all assets");
                    foreach (var address in addressesAll)
                    {
                        System.Console.WriteLine($"{address.Asset} : {address.Address}");
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                    throw;
                }
            }

            public void GetChartData()
            {
                var provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
                var pair = new AssetPair("BTC", "ETH", provider);

                var ctx = new OhlcContext(pair, TimeResolution.Day, new TimeRange(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, TimeResolution.Day));

                try
                {
                    var ohlc = AsyncContext.Run(() => provider.GetOhlcAsync(ctx));

                    foreach (var entry in ohlc)
                    {
                        System.Console.WriteLine($"{entry.DateTimeUtc}: {entry.High}, {entry.Low}, {entry.Open}, {entry.Close}");
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
