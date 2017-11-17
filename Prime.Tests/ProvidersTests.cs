using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;

namespace Prime.Tests
{
    [TestClass]
    public class ProvidersTests
    {
        [TestMethod]
        public async Task GetPricesFromProvidersTest()
        {
            var ctx = new PublicPriceContext("BTC_USD".ToAssetPairRaw());

            foreach (var provider in Networks.I.Providers.OfType<IDELETEPublicPriceProvider>())
            {
                try
                {
                    var r = await provider.GetPriceAsync(ctx);

                    Trace.WriteLine($"{r.QuoteAsset}: {r.Price.Display} - {provider.Network}");
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"{provider.Network} failed: {e.Message}");
                }
            }
        }
    }
}
