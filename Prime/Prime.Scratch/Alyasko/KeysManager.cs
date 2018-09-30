using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Prime.Core;

namespace Prime.Scratch.Frank.Tests.Alyasko
{
    /*
    public class KeysManager : ITestBase
    {
        public void Go()
        {
            System.Console.WriteLine("--- Keys Management Tool ---");

            var providers = Networks.I.Providers.AsEnumerable().FilterType<INetworkProvider, INetworkProviderPrivate>().Where(x => x.IsDirect).ToList();

            foreach (var provider in providers)
            {
                System.Console.WriteLine($"{provider.Network.Name}");
                var providerKey = UserContext.Testing.GetApiKey(provider);
                if(providerKey != null)
                    System.Console.WriteLine($"Key: {providerKey.Key}");

                System.Console.WriteLine();
            }

            System.Console.WriteLine("Enter exchange name to enter keys info:");
            var exhcnageName = System.Console.ReadLine();

            var exchange =
                providers.FirstOrDefault(x => x.Network.Name.Equals(exhcnageName, StringComparison.OrdinalIgnoreCase));

            if (exchange != null)
            {
                System.Console.WriteLine("Entering keys for: ");
                System.Console.WriteLine($"{exchange.Title}");

                System.Console.WriteLine("Enter Key:");
                var key = System.Console.ReadLine();

                System.Console.WriteLine("Enter Secret:");
                var secret = System.Console.ReadLine();

                var apiKey = new ApiKey(exchange.Network, "Default", key, secret);
                var keys = UserContext.Testing.ApiKeys;
                keys.RemoveNetwork(exchange.Network.Id);

                keys.Add(apiKey);
                keys.Save();
            }
        }
    }*/
}
