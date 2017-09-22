using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using plugins;
using Prime.Core;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class KrakenTests
        {
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
        }
    }
}
