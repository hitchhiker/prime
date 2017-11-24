using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Utility;

namespace Prime.Tests.Providers
{
    public abstract partial class ProviderDirectTestsBase
    {
        public virtual void TestGetPricing() { }
        public void TestGetPricing(List<AssetPair> pairs, bool firstPriceLessThan1)
        {
            var p = IsType<IPublicPricingProvider>();
            if (p.Success)
                GetPricing(p.Provider, pairs, firstPriceLessThan1);
        }

        private void InternalGetPriceAsync(IPublicPricingProvider provider, PublicPricesContext context, bool firstPriceLessThan1, bool runSingle)
        {
            Assert.IsTrue(provider.PricingFeatures != null, "Pricing features object is null");

            var r = AsyncContext.Run(() => provider.GetPricingAsync(context));

            Assert.IsTrue(runSingle ? r.WasViaSingleMethod : !r.WasViaSingleMethod,
                runSingle
                    ? "Single price request was completed using multiple prices endpoint"
                    : "Multiple price request was completed using single price endpoint");
            Assert.IsTrue(r.IsCompleted, "Request is not completed. Missing pairs: " + string.Join(",", r.MissedPairs));

            Assert.IsTrue(r.FirstPrice != null, "First price is null");

            if (context.IsRequestAll)
                Assert.IsTrue(!context.Pairs.Any(), "Context should not have any pairs when requesting prices for all supported by exchange pairs");
            else
            {
                Assert.IsTrue(r.FirstPrice.QuoteAsset.Equals(context.Pair.Asset1), "Incorrect base asset");
                Assert.IsTrue(r.FirstPrice.Price.Asset.Equals(context.Pair.Asset2), "Incorrect quote asset");

                Assert.IsTrue(r.MarketPrices.DistinctBy(x => x.Pair).Count() == context.Pairs.Count, "Pair duplicates found");
            }

            if (context.IsRequestAll == false)
            {
                if (firstPriceLessThan1) // Checks if the pair is reversed (price-wise).
                    Assert.IsTrue(r.FirstPrice.Price < 1, "Reverse check failed. Price is expected to be < 1");
                else
                    Assert.IsTrue(r.FirstPrice.Price > 1, "Reverse check failed. Price is expected to be > 1");
            }

            Trace.WriteLine($"First asset: {r.FirstPrice}");

            var pricingFeatures = runSingle ? provider.PricingFeatures.Single : provider.PricingFeatures.Bulk as PricingFeaturesItemBase;

            foreach (var p in r.MarketPrices)
            {
                Trace.WriteLine($"Market price: {p}");

                if (pricingFeatures.CanStatistics)
                {
                    Assert.IsTrue(p.HasStatistics,
                        $"Market price does not have statistics but provider supports it - {p.Pair}");

                    Trace.WriteLine($"Market price statistics for {p.Pair}:");

                    Trace.WriteLine(
                        $"Bid: {(p.PriceStatistics.HasHighestBid ? p.PriceStatistics.HighestBid.Display : "-")}");
                    Trace.WriteLine(
                        $"Ask: {(p.PriceStatistics.HasLowestAsk ? p.PriceStatistics.LowestAsk.Display : "-")}");
                    Trace.WriteLine(
                        $"Low: {(p.PriceStatistics.HasPrice24Low ? p.PriceStatistics.Price24Low.Display : "-")}");
                    Trace.WriteLine(
                        $"High: {(p.PriceStatistics.HasPrice24High ? p.PriceStatistics.Price24High.Display : "-")}");
                }
                else
                {
                    Assert.IsTrue(!p.HasStatistics, $"Provider returns statistics but did not announce it - {p.Pair}");
                }

                if (pricingFeatures.CanVolume)
                {
                    Assert.IsTrue(p.HasVolume,
                        $"Market price does not have volume but provider supports it - {p.Pair}");

                    if (p.Volume.HasVolume24Base)
                        Trace.WriteLine($"Base 24h volume: {p.Volume.Volume24Base}");

                    if (p.Volume.HasVolume24Quote)
                        Trace.WriteLine($"Quote 24h volume: {p.Volume.Volume24Quote}");
                }
                else
                {
                    Assert.IsTrue(!p.HasVolume, $"Provider returns volume but did not announce it - {p.Pair}");
                }

                Trace.WriteLine("");
            }
        }

        private void GetPricing(IPublicPricingProvider provider, List<AssetPair> pairs, bool firstPriceLessThan1)
        {
            try
            {
                Trace.WriteLine("Pricing interface test\n\n");

                if (provider.PricingFeatures.HasSingle)
                {
                    Trace.WriteLine("\nSingle features test\n");
                    var context = new PublicPriceContext(pairs.First())
                    {
                        RequestStatistics = provider.PricingFeatures.Single.CanStatistics,
                        RequestVolume = provider.PricingFeatures.Single.CanVolume
                    };

                    InternalGetPriceAsync(provider, context, firstPriceLessThan1, true);
                }

                if (provider.PricingFeatures.HasBulk)
                {
                    Trace.WriteLine("\nBulk features test with pairs selection\n");
                    var context = new PublicPricesContext(pairs)
                    {
                        RequestStatistics = provider.PricingFeatures.Bulk.CanStatistics,
                        RequestVolume = provider.PricingFeatures.Bulk.CanVolume
                    };

                    InternalGetPriceAsync(provider, context, firstPriceLessThan1, false);

                    if (provider.PricingFeatures.Bulk.CanReturnAll)
                    {
                        Trace.WriteLine("\nBulk features test (provider can return all prices)\n");
                        context = new PublicPricesContext();

                        InternalGetPriceAsync(provider, context, firstPriceLessThan1, false);
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
