using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nito.AsyncEx;
using Prime.Common;

namespace Prime.Core.Market
{
    public class VolumeProvider
    {
        public readonly IAggVolumeDataProvider ProviderAggVolumeData;
        public readonly IReadOnlyList<IPublicVolumeProvider> ProvidersPublicVolume;
        public readonly IReadOnlyList<IPublicPricingProvider> ProvidersPublicPricing;

        public static VolumeProvider I => Lazy.Value;
        private static readonly Lazy<VolumeProvider> Lazy = new Lazy<VolumeProvider>(() => new VolumeProvider());

        private VolumeProvider()
        {
            ProviderAggVolumeData = Networks.I.Providers.OfType<IAggVolumeDataProvider>().FirstOrDefault();
            ProvidersPublicVolume = Networks.I.Providers.OfType<IPublicVolumeProvider>().Where(x => x.IsDirect).ToList();
            ProvidersPublicPricing = Networks.I.Providers.OfType<IPublicPricingProvider>().Where(x => x.IsDirect && x.PricingFeatures.HasVolume).ToList();
        }

        public PublicVolumeResponse Get(Network network)
        {
            var pp = network.PublicPriceProviders.FirstOrDefault(x => x.PricingFeatures.HasVolume && x.IsDirect);
            var pv = network.PublicVolumeProviders.FirstOrDefault(x => x.IsDirect);

            var pricing = pp?.PricingFeatures;
            var volume = pv?.VolumeFeatures;

            if (volume?.HasBulk == true && volume.Bulk.CanReturnAll)
            {
                var vr = ApiCoordinator.GetPublicVolume(pv, new PublicVolumesContext());
                return vr.IsNull ? null : vr.Response;
            }

            if (pricing?.HasVolume == true && pricing.HasBulk && pricing.Bulk.CanReturnAll)
            {
                var pr = ApiCoordinator.GetPricing(pp, new PublicPricesContext());
                return pr.IsNull ? null : new PublicVolumeResponse(pp.Network, pr.Response);
            }

            var pairs = AsyncContext.Run(() => AssetPairProvider.I.GetPairsAsync(network));

            return Get(network, pairs);
        }

        public PublicVolumeResponse Get(Network network, IEnumerable<AssetPair> pairs)
        {
            var pp = network.PublicPriceProviders.FirstOrDefault(x => x.PricingFeatures.HasVolume && x.IsDirect);
            var pv = network.PublicVolumeProviders.FirstOrDefault(x => x.IsDirect);

            var pricing = pp?.PricingFeatures;
            var volume = pv?.VolumeFeatures;

            if (volume?.HasBulk == true)
            {
                var vr = ApiCoordinator.GetPublicVolume(pv, new PublicVolumesContext(pairs.ToList()));
                return vr.IsNull ? null : vr.Response;
            }

            if (pricing?.HasBulk == true)
            {
                var pr = ApiCoordinator.GetPricing(pp, new PublicPricesContext(pairs.ToList()));
                return pr.IsNull ? null : new PublicVolumeResponse(pp.Network, pr.Response);
            }
        }

        public PublicVolumeResponse Get(Network network, AssetPair pair)
        {
            return null;
        }
    }
}
