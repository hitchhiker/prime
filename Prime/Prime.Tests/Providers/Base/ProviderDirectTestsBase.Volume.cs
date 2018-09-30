using System.Collections.Generic;
using System.Linq;
using Nito.AsyncEx;
using Prime.Core;
using Prime.Finance;
using Xunit;

namespace Prime.Tests.Providers
{
    public abstract partial class ProviderDirectTestsBase
    {
        public virtual void TestGetVolume() { }
        public void PretestGetVolume(List<AssetPair> pairs, bool firstVolumeBaseBiggerThanQuote)
        {
            var p = IsType<IPublicVolumeProvider>();
            if (p.Success)
                GetVolumeTest(p.Provider, pairs, firstVolumeBaseBiggerThanQuote);
        }

        private void InternalGetVolumeAsync(IPublicVolumeProvider provider, PublicVolumesContext context, bool firstVolumeBaseBiggerThanQuote, bool runSingle)
        {
            Assert.True(provider.VolumeFeatures != null, "Volume features object is null");

            var r = AsyncContext.Run(() => provider.GetPublicVolumeAsync(context));

            // First volume, IsRequestAll.
            var volume = r.Volume.FirstOrDefault();
            Assert.True(volume != null, "Provider returned no volume records");

            if (!context.IsRequestAll)
            {
                var dist = r.Volume.DistinctBy(x => x.Pair).ToList();
                Assert.True(context.Pairs.Count == dist.Count, "Provider didn't return required pairs");

                Assert.True(volume.Pair.Asset1.Equals(context.Pair.Asset1), "Incorrect base asset");
                Assert.True(volume.Pair.Asset2.Equals(context.Pair.Asset2), "Incorrect quote asset");
            }

            // First volume, base/quote volumes relation.
            if (volume.HasVolume24Base && volume.HasVolume24Quote)
            {
                if (firstVolumeBaseBiggerThanQuote)
                    Assert.True(volume.Volume24Base.ToDecimal(null) > volume.Volume24Quote.ToDecimal(null), "Quote volume is bigger than base (within volume)");
                else
                    Assert.True(volume.Volume24Base.ToDecimal(null) < volume.Volume24Quote.ToDecimal(null), "Base volume is bigger than quote (within volume)");
            }

            // All volume.
            foreach (var networkPairVolume in r.Volume)
            {
                if (networkPairVolume.HasVolume24Base)
                {
                    Assert.False(networkPairVolume.Volume24Base.ToDecimal(null) == 0,
                        $"Base volume of {networkPairVolume.Pair} is 0");
                    OutputWriter.WriteLine($"Base volume for {networkPairVolume.Pair} pair is {networkPairVolume.Volume24Base}");
                }

                if (networkPairVolume.HasVolume24Quote)
                {
                    Assert.False(networkPairVolume.Volume24Quote.ToDecimal(null) == 0,
                        $"Quote volume of {networkPairVolume.Pair} is 0");
                    OutputWriter.WriteLine($"Quote volume for {networkPairVolume.Pair} pair is {networkPairVolume.Volume24Quote}");
                }

                if (networkPairVolume.HasVolume24Base && networkPairVolume.HasVolume24Quote)
                {
                    Assert.True(volume.Volume24Base.ToDecimal(null) != volume.Volume24Quote.ToDecimal(null), "Base and quote volume are the same");
                }
            }
        }

        private void GetVolumeTest(IPublicVolumeProvider provider, List<AssetPair> pairs, bool volumeBaseBiggerThanQuote)
        {
            OutputWriter.WriteLine("Volume interface test\n\n");

            if (provider.VolumeFeatures.HasSingle)
            {
                OutputWriter.WriteLine("\nSingle features test\n");

                var context = new PublicVolumeContext(pairs.First());

                InternalGetVolumeAsync(provider, context, volumeBaseBiggerThanQuote, true);
            }

            if (provider.VolumeFeatures.HasBulk)
            {
                OutputWriter.WriteLine("\nBulk features test with pairs selection\n");
                var context = new PublicVolumesContext(pairs);

                InternalGetVolumeAsync(provider, context, volumeBaseBiggerThanQuote, false);

                if (provider.VolumeFeatures.Bulk.CanReturnAll)
                {
                    OutputWriter.WriteLine("\nBulk features test (provider can return all volumes)\n");
                    context = new PublicVolumesContext();

                    InternalGetVolumeAsync(provider, context, volumeBaseBiggerThanQuote, false);
                }
            }

            TestVolumePricingSanity(provider, pairs);
        }
    }
}
