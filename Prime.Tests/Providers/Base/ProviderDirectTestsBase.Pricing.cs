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
        public void TestGetPricing(List<AssetPair> pairs, bool firstPriceLessThan1, bool? firstVolumeBaseBiggerThanQuote = null)
        {
            var p = IsType<IPublicPricingProvider>();
            if (p.Success)
                GetPricing(p.Provider, pairs, firstPriceLessThan1, firstVolumeBaseBiggerThanQuote);
        }

        private void TestVolumesRelationWithinPricing(MarketPrice firstPrice, bool? firstVolumeBaseBiggerThanQuote)
        {
            if (!firstVolumeBaseBiggerThanQuote.HasValue)
                Assert.Fail("Relation of base and quote volumes is not set even though provider returned them within pricing request");

            Assert.IsTrue(firstPrice.Volume.Volume24Base.ToDecimal(null) != firstPrice.Volume.Volume24Quote.ToDecimal(null), "Base and quote volumes are same (within pricing)");

            if (firstVolumeBaseBiggerThanQuote.Value)
                Assert.IsTrue(firstPrice.Volume.Volume24Base.ToDecimal(null) > firstPrice.Volume.Volume24Quote.ToDecimal(null), "Quote volume is bigger than base (within pricing)");
            else
                Assert.IsTrue(firstPrice.Volume.Volume24Base.ToDecimal(null) < firstPrice.Volume.Volume24Quote.ToDecimal(null), "Base volume is bigger than quote (within pricing)");
        }

        private void InternalGetPriceAsync(IPublicPricingProvider provider, PublicPricesContext context, bool runSingle, bool firstPriceLessThan1, bool? firstVolumeBaseBiggerThanQuote = null)
        {
            Assert.IsTrue(provider.PricingFeatures != null, "Pricing features object is null");

            var r = AsyncContext.Run(() => provider.GetPricingAsync(context));

            // General.
            Assert.IsTrue(runSingle ? r.WasViaSingleMethod : !r.WasViaSingleMethod,
                runSingle
                    ? "Single price request was completed using multiple prices endpoint"
                    : "Multiple price request was completed using single price endpoint");

            Assert.IsTrue(r.IsCompleted, "Request is not completed. Missing pairs: " + string.Join(", ", r.MissedPairs));

            // Multiple prices.
            if (context.IsRequestAll)
                Assert.IsTrue(!context.Pairs.Any(),
                    "Context should not have any pairs when requesting prices for all supported by exchange pairs");
            else
                Assert.IsTrue(r.MarketPrices.DistinctBy(x => x.Pair).Count() == context.Pairs.Count,
                    "Pair duplicates found");

            // First price, price value.
            Assert.IsTrue(r.FirstPrice != null, "First price is null");
            Trace.WriteLine($"First asset price: {r.FirstPrice}");

            if (!context.IsRequestAll)
            {
                Assert.IsTrue(r.FirstPrice.QuoteAsset.Equals(context.Pair.Asset1), "Incorrect base asset");
                Assert.IsTrue(r.FirstPrice.Price.Asset.Equals(context.Pair.Asset2), "Incorrect quote asset");

                if (firstPriceLessThan1) // Checks if the pair is reversed (price-wise).
                    Assert.IsTrue(r.FirstPrice.Price < 1, "Reverse check failed. Price is expected to be < 1");
                else
                    Assert.IsTrue(r.FirstPrice.Price > 1, "Reverse check failed. Price is expected to be > 1");
            }

            // First price. Volume base/quote relation.
            var canAllVolume = r.FirstPrice.HasVolume && r.FirstPrice.Volume.HasVolume24Base && r.FirstPrice.Volume.HasVolume24Quote;

            if (provider.PricingFeatures.HasSingle && provider.PricingFeatures.Single.CanVolume && canAllVolume)
                TestVolumesRelationWithinPricing(r.FirstPrice, firstVolumeBaseBiggerThanQuote);
            if (provider.PricingFeatures.HasBulk && provider.PricingFeatures.Bulk.CanVolume && canAllVolume)
                TestVolumesRelationWithinPricing(r.FirstPrice, firstVolumeBaseBiggerThanQuote);

            // All market prices and pricing features.
            var pricingFeatures = runSingle ? provider.PricingFeatures.Single : provider.PricingFeatures.Bulk as PricingFeaturesItemBase;

            foreach (var p in r.MarketPrices)
            {
                Trace.WriteLine($"Market price: {p}");

                // Statistics.
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

                // Volume.
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

        private void GetPricing(IPublicPricingProvider provider, List<AssetPair> pairs, bool firstPriceLessThan1, bool? firstVolumeBaseBiggerThanQuote = null)
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

                    InternalGetPriceAsync(provider, context, true, firstPriceLessThan1, firstVolumeBaseBiggerThanQuote);
                }

                if (provider.PricingFeatures.HasBulk)
                {
                    Trace.WriteLine("\nBulk features test with pairs selection\n");
                    var context = new PublicPricesContext(pairs)
                    {
                        RequestStatistics = provider.PricingFeatures.Bulk.CanStatistics,
                        RequestVolume = provider.PricingFeatures.Bulk.CanVolume
                    };

                    InternalGetPriceAsync(provider, context, false, firstPriceLessThan1, firstVolumeBaseBiggerThanQuote);

                    if (provider.PricingFeatures.Bulk.CanReturnAll)
                    {
                        Trace.WriteLine("\nBulk features test (provider can return all prices)\n");
                        context = new PublicPricesContext();

                        InternalGetPriceAsync(provider, context, false, firstPriceLessThan1, firstVolumeBaseBiggerThanQuote);
                    }
                }

                TestVolumePricingSanity(provider, pairs);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
