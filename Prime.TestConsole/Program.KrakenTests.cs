using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using plugins;
using Prime.Common;
using Prime.Plugins.Services.Kraken;
using AssetPair = Prime.Common.AssetPair;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class KrakenTests
        {
            public void GetDepositAddresses()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();

                var ctx = new WalletAddressAssetContext("BTC".ToAsset(provider), UserContext.Current);

                try
                {
                    var addresses = AsyncContext.Run(() => provider.GetAddressesForAssetAsync(ctx));

                    foreach (var walletAddress in addresses)
                    {
                        Console.WriteLine($"{walletAddress.Asset} : {walletAddress.Address}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void GetLatestPrice()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
                var ctx = new PublicPriceContext(new AssetPair("BTC", "USD"));

                try
                {
                    var price = AsyncContext.Run(() => provider.GetPriceAsync(ctx));

                    Console.WriteLine($"Latest {ctx.Pair} value is {price.Price}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void GetFundingMethod()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();

                var ctx = new NetworkProviderPrivateContext(UserContext.Current);

                try
                {
                    var method = AsyncContext.Run(() => provider.GetFundingMethod(ctx, Asset.Btc));

                    Console.WriteLine($"Funding method: {method}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    // throw;
                }
            }

            public void GetAssetPairs()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
                var ctx = new NetworkProviderPrivateContext(UserContext.Current);

                var pairs = AsyncContext.Run(() => provider.GetAssetPairsAsync(ctx));

                try
                {
                    foreach (var pair in pairs)
                    {
                        Console.WriteLine($"{pair}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void GetBalances()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
                var ctx = new NetworkProviderPrivateContext(UserContext.Current);

                try
                {
                    var balances = AsyncContext.Run(() => provider.GetBalancesAsync(ctx));

                    if (balances.Count == 0)
                    {
                        Console.WriteLine("No balances.");
                    }
                    else
                    {
                        foreach (var balance in balances)
                        {
                            Console.WriteLine(
                                $"{balance.Asset}: {balance.Available}, {balance.Balance}, {balance.Reserved}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void GetOhlc()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();

                var ctx = new OhlcContext(new AssetPair("BTC", "USD"), TimeResolution.Minute, null, null);

                try
                {
                    var ohlc = AsyncContext.Run(() => provider.GetOhlcAsync(ctx));

                    foreach (var data in ohlc)
                    {
                        Console.WriteLine($"{data.DateTimeUtc}: {data.High} {data.Low}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void TestApi()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();

                var ctx = new ApiPrivateTestContext(UserContext.Current.GetApiKey(provider));

                try
                {
                    var result = AsyncContext.Run(() => provider.TestPrivateApiAsync(ctx));

                    Console.WriteLine($"Api test ok: {result}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }

            }

            public void GetAllAddressesAsync()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();

                var ctx = new WalletAddressContext(UserContext.Current);

                try
                {
                    var addresses = AsyncContext.Run(() => provider.GetAddressesAsync(ctx));

                    foreach (var walletAddress in addresses)
                    {
                        Console.WriteLine($"{walletAddress.Asset} : {walletAddress.Address}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    // throw;
                }
            }
        }
    }
}
