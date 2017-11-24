using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
using Prime.Common;

namespace Prime.Tests.Providers
{
    public abstract partial class ProviderDirectTestsBase
    {

        public virtual void TestGetVolume() { }
        public void TestGetVolume(List<AssetPair> pairs)
        {
            var p = IsType<IPublicVolumeProvider>();
            if (p.Success)
                GetVolume(p.Provider, pairs);
        }

        private void InternalGetVolumeAsync(IPublicVolumeProvider provider, PublicVolumesContext context, bool runSingle)
        {
            Assert.IsTrue(provider.VolumeFeatures != null, "Volume features object is null");

            var r = AsyncContext.Run(() => provider.GetPublicVolumeAsync(context));
        }

        private void GetVolume(IPublicVolumeProvider provider, List<AssetPair> pairs)
        {
            try
            {
                Trace.WriteLine("Volume interface test\n\n");

                if (provider.VolumeFeatures.HasSingle)
                {
                    Trace.WriteLine("\nSingle features test\n");

                    var context = new PublicVolumeContext(pairs.First());

                    InternalGetVolumeAsync(provider, context, true);
                }

                if (provider.VolumeFeatures.HasBulk)
                {
                    Trace.WriteLine("\nBulk features test with pairs selection\n");
                    var context = new PublicVolumesContext(pairs);

                    InternalGetVolumeAsync(provider, context, false);

                    if (provider.VolumeFeatures.Bulk.CanReturnAll)
                    {
                        Trace.WriteLine("\nBulk features test (provider can return all volumes)\n");
                        context = new PublicVolumesContext();

                        InternalGetVolumeAsync(provider, context, false);
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
