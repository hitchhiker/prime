using System;
using System.Diagnostics;
using System.Linq;
using Nito.AsyncEx;
using Prime.Common;
using Xunit;

namespace Prime.Tests
{
    public class ProvidersTests
    {
        [Fact]
        public void TestProviderFeatureSanity()
        {
            foreach (var prov in Networks.I.Providers.OfType<IPublicVolumeProvider>())
            {
                Assert.True(prov.VolumeFeatures != null, prov.Title + " volume features is null.");
            }

            foreach (var prov in Networks.I.Providers.OfType<IPublicPricingProvider>())
            {
                Assert.True(prov.PricingFeatures != null, prov.Title + " pricing features is null.");
            }
        }

        [Fact]
        public void GetVolumeAndPricingProviders()
        {
            var providers = Networks.I.Providers.OfType<IPublicPricingProvider>().OfType<IPublicVolumeProvider>();

            foreach (var provider in providers)
            {
                Trace.WriteLine($"Provider: {provider.Network.Name}");
            }
        }

        [Fact]
        public void GetPricesFromProvidersTest()
        {
            // Sean check pricing method of your providers here.

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
