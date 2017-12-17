using System.Linq;
using Prime.Common;

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

        public void ListOrderBookProviders()
        {
            System.Console.WriteLine("Providers that support order book getting:");

            var providers = Networks.I.Providers.OfType<IOrderBookProvider>().ToArray();

            foreach (var bookProvider in providers)
            {
                System.Console.WriteLine($"{bookProvider.Network.Name}");
            }
        }

        public void Go()
        {
            //GenerateProvidersReport();
            ListOrderBookProviders();
        }
    }
}
