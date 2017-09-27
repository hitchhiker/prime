using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KrakenApi;
using Nito.AsyncEx;
using plugins;
using Prime.Core;
using Prime.Plugins.Services.Kraken;
using AssetPair = Prime.Core.AssetPair;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class KrakenTests
        {
            public void GetDepositAddresses()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
                var privateProvider = new NetworkProviderPrivateContext(UserContext.Current);

                var ctx = new WalletAddressAssetContext("BTC".ToAsset(provider), false, UserContext.Current);

                var userCtx = UserContext.Current;
                var apiKey = userCtx.GetApiKey(provider);

                // BUG: this always returns EInternal Error.
                // var kraken = new Kraken(apiKey.Key, apiKey.Secret);
                // var methods = kraken.GetDepositMethods(null, "XBT");

                try
                {
                    var addresses = AsyncContext.Run(() => provider.GetAddressesForAssetAsync(ctx));
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

                var price = AsyncContext.Run(() => provider.GetLatestPriceAsync(ctx));

                try
                {
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

                if (false)
                {
					// BUG: this code throws exception "EGeneral: Internal error".

                    var apiKey = UserContext.Current.ApiKeys.GetFirst(provider);
                    var kraken = new Kraken(apiKey.Key, apiKey.Secret);
                    var m = kraken.GetDepositMethods(null, "XBT");
                }

                var ctx = new NetworkProviderPrivateContext(UserContext.Current);

                // BUG: this code also throws exception "EGeneral: Internal error". Needs to be checked with other keys.
                var method = AsyncContext.Run(() => provider.GetFundingMethod(ctx, Asset.Btc));

                try
                {
                    Console.WriteLine($"Funding method: {method}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public void GetAssetPairs()
            {
                var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
                var ctx = new NetworkProviderPrivateContext(UserContext.Current);

                var pairs = AsyncContext.Run(() => provider.GetAssetPairs(ctx));

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
                try
                {
                    var provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
                    var ctx = new NetworkProviderPrivateContext(UserContext.Current);

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

                var ohlc = AsyncContext.Run(() => provider.GetOhlcAsync(ctx));

                try
                {
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
        }
    }
}
