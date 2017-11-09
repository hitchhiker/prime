using System;
using System.Linq;
using Prime.Common;

namespace Prime.TestConsole.Tools
{
    public class ProviderTools
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
    }
}
