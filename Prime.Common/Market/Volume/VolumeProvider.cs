using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var prov = network.PublicPriceProviders.FirstOrDefault(x => x.PricingFeatures.HasVolume && x.IsDirect);
            if (prov != null)
            {
                var f = prov.PricingFeatures;
                if (f.HasVolume && f.HasBulk && f.Bulk.CanReturnAll)
                {
                    ApiCoordinator.GetPricing(prov, new PublicPricesContext());
                }
                AssetPairProvider.I.GetNetworkPairsAsync();
            }
            return null;
        }

        public PublicVolumeResponse Get(Network network, AssetPair pair)
        {
            return null;
        }
    }
}
