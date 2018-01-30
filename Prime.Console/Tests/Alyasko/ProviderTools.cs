using System;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Console.Tests.Alyasko
{
    public class ProviderTools : ITestBase
    {
        public void GenerateProvidersReport()
        {
            var providers = Networks.I.Providers.Where(x => x.IsDirect).OrderBy(x => x.Network.Name).ToArray();

            var providerInfos = providers.Select(x => new ProviderInfo(x));

            foreach (var info in providerInfos)
            {
                info.PrintReadableInfo();
            }
        }

        public void ListPrivateProviders()
        {
            System.Console.WriteLine("Providers that implement private API:");

            var providers = Networks.I.Providers.OfType<INetworkProviderPrivate>().ToArray();
            providers.ForEach(x =>
            {
                System.Console.WriteLine($"{x.Network.Name}");
            });
        }

        public void ListOrderBookProviders()
        {
            System.Console.WriteLine("Providers that support order book getting:");

            var providers = Networks.I.Providers.OfType<IOrderBookProvider>().ToArray();

            foreach (var bookProvider in providers)
            {
                System.Console.WriteLine($"{bookProvider.Network.Name}");
            }
        }

        public void ListProvidersThatSupport(Asset asset)
        {
            System.Console.WriteLine($"Providers that support {asset} asset:");

            var providers = Networks.I.Providers.OfType<IAssetPairsProvider>().ToArray();

            foreach (var provider in providers)
            {
                try
                {
                    var suppoertedPairs = provider.GetAssetPairsAsync(new NetworkProviderContext()).Result;
                    if (suppoertedPairs.Any(x => x.Has(asset)))
                        System.Console.WriteLine($"{provider.Network.Name} supports {asset}");
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"Error in {provider.Network.Name}: {e.Message}");
                }
            }
        }

        public void Go()
        {
            //GenerateProvidersReport();
            // ListOrderBookProviders();
            ListPrivateProviders();
            //ListProvidersThatSupport("XRP".ToAssetRaw());
        }
    }
}
