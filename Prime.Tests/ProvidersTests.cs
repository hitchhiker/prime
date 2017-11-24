using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
using Prime.Common;

namespace Prime.Tests
{
    [TestClass]
    public class ProvidersTests
    {
        [TestMethod]
        public void TestProviderFeatureSanity()
        {
            foreach (var prov in Networks.I.Providers.OfType<IPublicVolumeProvider>())
            {
                Assert.IsNotNull(prov.VolumeFeatures, prov.Title + " volume features is null.");
            }

            foreach (var prov in Networks.I.Providers.OfType<IPublicPricingProvider>())
            {
                Assert.IsNotNull(prov.PricingFeatures, prov.Title + " pricing features is null.");
            }
        }

        [TestMethod]
        public void GetVolumeAndPricingProviders()
        {
            var providers = Networks.I.Providers.OfType<IPublicPricingProvider>().OfType<IPublicVolumeProvider>();

            foreach (var provider in providers)
            {
                Trace.WriteLine($"Provider: {provider.Network.Name}");
            }
        }

        [TestMethod]
        public void GetPricesFromProvidersTest()
        {
            // TODO: Sean check your providers here.

            var ctx = new PublicPriceContext("BTC_USD".ToAssetPairRaw());

            var providers = Networks.I.Providers.OfType<IPublicPricingProvider>().Where(x => x.IsDirect).ToList();

            foreach (var provider in providers)
            {
                try
                {
                    var r = AsyncContext.Run(() => provider.GetPricingAsync(ctx)).FirstPrice;

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
