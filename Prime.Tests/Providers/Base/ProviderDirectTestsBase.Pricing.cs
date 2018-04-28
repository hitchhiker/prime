using System.Collections.Generic;
using System.Linq;
using Nito.AsyncEx;
using Prime.Common;
using Xunit;

namespace Prime.Tests.Providers
{
    public abstract partial class ProviderDirectTestsBase
    {
        public virtual void TestGetPricing() { }
        public void PretestGetPricing(List<AssetPair> pairs, bool firstPriceLessThan1, bool? firstVolumeBaseBiggerThanQuote = null)
        {
            var p = IsType<IPublicPricingProvider>();
            if (p.Success)
                GetPricingTest(p.Provider, pairs, firstPriceLessThan1, firstVolumeBaseBiggerThanQuote);
        }

        private void TestVolumesRelationWithinPricing(MarketPrice firstPrice, bool? firstVolumeBaseBiggerThanQuote)
        {
            if (!firstVolumeBaseBiggerThanQuote.HasValue)
                Assert.True(false, "Relation of base and quote volumes is not set even though provider returned them within pricing request");

            Assert.True(firstPrice.Volume.Volume24Base.ToDecimal(null) != firstPrice.Volume.Volume24Quote.ToDecimal(null), "Base and quote volumes are same (within pricing)");

            if (firstVolumeBaseBiggerThanQuote.Value)
                Assert.True(firstPrice.Volume.Volume24Base.ToDecimal(null) > firstPrice.Volume.Volume24Quote.ToDecimal(null), "Quote volume is bigger than base (within pricing)");
            else
                Assert.True(firstPrice.Volume.Volume24Base.ToDecimal(null) < firstPrice.Volume.Volume24Quote.ToDecimal(null), "Base volume is bigger than quote (within pricing)");
        }

        private void InternalGetPriceAsync(IPublicPricingProvider provider, PublicPricesContext context, bool runSingle, bool firstPriceLessThan1, bool? firstVolumeBaseBiggerThanQuote = null)
        {
            Assert.True(provider.PricingFeatures != null, "Pricing features object is null");

            var r = AsyncContext.Run(() => provider.GetPricingAsync(context));

            // General.
            Assert.True(runSingle ? r.WasViaSingleMethod : !r.WasViaSingleMethod,
                runSingle
                    ? "Single price request was completed using multiple prices endpoint"
                    : "Multiple price request was completed using single price endpoint");

            Assert.True(r.IsCompleted, "Request is not completed. Missing pairs: " + string.Join(", ", r.MissedPairs));

            var dist = r.DistinctBy(x => x.Pair).ToList();
            
            // Multiple prices.
            if (context.IsRequestAll)
                Assert.True(!context.Pairs.Any(),
                    "Context should not have any pairs when requesting prices for all supported by exchange pairs");
            else
                Assert.True(r.DistinctBy(x => x.Pair).Count() == context.Pairs.Count,
                    "Number of returned pairs is not equal to requested");

            // First price, price value.
            Assert.True(r.FirstPrice != null, "First price is null");
            OutputWriter.WriteLine($"First asset price: {r.FirstPrice}");

            if (!context.IsRequestAll)
            {
                var firstPrice = r.FirstPrice;

                Assert.True(firstPrice.QuoteAsset.Equals(context.Pair.Asset1), "Incorrect base asset");
                Assert.True(firstPrice.Price.Asset.Equals(context.Pair.Asset2), "Incorrect quote asset");

                // Checks if the pair is reversed (price-wise).
                if (firstPriceLessThan1)
                    Assert.True(firstPrice.Price < 1, "Reverse check failed. Price is expected to be < 1");
                else
                    Assert.True(firstPrice.Price > 1, "Reverse check failed. Price is expected to be > 1");

                // Checks if statistics price are correct.
                if (firstPrice.HasStatistics)
                {
                    if (firstPrice.PriceStatistics.HasHighestBid)
                    {
                        if (firstPriceLessThan1)
                            Assert.True(firstPrice.PriceStatistics.HighestBid < 1,
                                "Reverse check failed. Highest bid price is expected to be < 1");
                        else
                            Assert.True(firstPrice.PriceStatistics.HighestBid > 1,
                                "Reverse check failed. Highest bid price is expected to be > 1");
                    }

                    if (firstPrice.PriceStatistics.HasLowestAsk)
                    {
                        if (firstPriceLessThan1)
                            Assert.True(firstPrice.PriceStatistics.LowestAsk < 1,
                                "Reverse check failed. Lowest ask price is expected to be < 1");
                        else
                            Assert.True(firstPrice.PriceStatistics.LowestAsk > 1,
                                "Reverse check failed. Lowest ask price is expected to be > 1");
                    }

                    if (firstPrice.PriceStatistics.HasPrice24High)
                    {
                        if (firstPriceLessThan1)
                            Assert.True(firstPrice.PriceStatistics.Price24High < 1,
                                "Reverse check failed. Highest 24h price is expected to be < 1");
                        else
                            Assert.True(firstPrice.PriceStatistics.Price24High > 1,
                                "Reverse check failed. Highest 24h price is expected to be > 1");
                    }

                    if (firstPrice.PriceStatistics.HasPrice24Low)
                    {
                        if (firstPriceLessThan1)
                            Assert.True(firstPrice.PriceStatistics.Price24Low < 1,
                            "Reverse check failed. Lowest 24h price is expected to be < 1");
                        else
                            Assert.True(firstPrice.PriceStatistics.Price24Low > 1,
                                "Reverse check failed. Lowest 24h price is expected to be > 1");
                    }

                    // Check if 24 highest >= 24 lowest.
                    if (firstPrice.PriceStatistics.HasPrice24Low && firstPrice.PriceStatistics.HasPrice24High)
                        Assert.True(firstPrice.PriceStatistics.Price24High >= firstPrice.PriceStatistics.Price24Low, "24h highest price is smaller that 24h lowest price");
                }

                // First price. Volume base/quote relation.
                var canAllVolume = r.FirstPrice.HasVolume && r.FirstPrice.Volume.HasVolume24Base && r.FirstPrice.Volume.HasVolume24Quote;

                if (provider.PricingFeatures.HasSingle && provider.PricingFeatures.Single.CanVolume && canAllVolume)
                    TestVolumesRelationWithinPricing(r.FirstPrice, firstVolumeBaseBiggerThanQuote);
                if (provider.PricingFeatures.HasBulk && provider.PricingFeatures.Bulk.CanVolume && canAllVolume)
                    TestVolumesRelationWithinPricing(r.FirstPrice, firstVolumeBaseBiggerThanQuote);
            }

            // All market prices and pricing features.
            var pricingFeatures = runSingle ? provider.PricingFeatures.Single : provider.PricingFeatures.Bulk as PricingFeaturesItemBase;

            foreach (var p in r)
            {
                OutputWriter.WriteLine($"Market price: {p}");

                // Statistics.
                if (pricingFeatures.CanStatistics)
                {
                    Assert.True(p.HasStatistics,
                        $"Market price does not have statistics but provider supports it - {p.Pair}");

                    OutputWriter.WriteLine($"Market price statistics for {p.Pair}:");

                    OutputWriter.WriteLine(
                        $"Bid: {(p.PriceStatistics.HasHighestBid ? p.PriceStatistics.HighestBid.Display : "-")}");
                    OutputWriter.WriteLine(
                        $"Ask: {(p.PriceStatistics.HasLowestAsk ? p.PriceStatistics.LowestAsk.Display : "-")}");
                    OutputWriter.WriteLine(
                        $"Low: {(p.PriceStatistics.HasPrice24Low ? p.PriceStatistics.Price24Low.Display : "-")}");
                    OutputWriter.WriteLine(
                        $"High: {(p.PriceStatistics.HasPrice24High ? p.PriceStatistics.Price24High.Display : "-")}");
                }
                else
                {
                    Assert.True(!p.HasStatistics, $"Provider returns statistics but did not announce it - {p.Pair}");
                }

                // Volume.
                if (pricingFeatures.CanVolume)
                {
                    Assert.True(p.HasVolume,
                        $"Market price does not have volume but provider supports it - {p.Pair}");

                    if (p.Volume.HasVolume24Base)
                        OutputWriter.WriteLine($"Base 24h volume: {p.Volume.Volume24Base}");

                    if (p.Volume.HasVolume24Quote)
                        OutputWriter.WriteLine($"Quote 24h volume: {p.Volume.Volume24Quote}");
                }
                else
                {
                    Assert.True(!p.HasVolume, $"Provider returns volume but did not announce it - {p.Pair}");
                }

                OutputWriter.WriteLine("");
            }
        }

        protected void GetPricingTest(IPublicPricingProvider provider, List<AssetPair> pairs, bool firstPriceLessThan1, bool? firstVolumeBaseBiggerThanQuote = null)
        {
            OutputWriter.WriteLine("Pricing interface test\n\n");

            if (provider.PricingFeatures.HasSingle)
            {
                OutputWriter.WriteLine("\nSingle features test\n");
                var context = new PublicPriceContext(pairs.First())
                {
                    RequestStatistics = provider.PricingFeatures.Single.CanStatistics,
                    RequestVolume = provider.PricingFeatures.Single.CanVolume
                };

                InternalGetPriceAsync(provider, context, true, firstPriceLessThan1, firstVolumeBaseBiggerThanQuote);
            }

            if (provider.PricingFeatures.HasBulk)
            {
                OutputWriter.WriteLine("\nBulk features test with pairs selection\n");
                var context = new PublicPricesContext(pairs)
                {
                    RequestStatistics = provider.PricingFeatures.Bulk.CanStatistics,
                    RequestVolume = provider.PricingFeatures.Bulk.CanVolume
                };

                InternalGetPriceAsync(provider, context, false, firstPriceLessThan1, firstVolumeBaseBiggerThanQuote);

                if (provider.PricingFeatures.Bulk.CanReturnAll)
                {
                    OutputWriter.WriteLine("\nBulk features test (provider can return all prices)\n");
                    context = new PublicPricesContext();

                    InternalGetPriceAsync(provider, context, false, firstPriceLessThan1, firstVolumeBaseBiggerThanQuote);
                }
            }

            TestVolumePricingSanity(provider, pairs);
        }
    }
}
