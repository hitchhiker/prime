using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Utility;

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

        public async Task<PublicVolumeResponse> GetAsync(Network network, VolumeProviderContext context = null)
        {
            var pv = network.PublicVolumeProviders.FirstOrDefault(x => x.IsDirect);
            var volume = pv?.VolumeFeatures;

            if (volume?.HasBulk == true && volume.Bulk.CanReturnAll)
            {
                var vr = await ApiCoordinator.GetPublicVolumeAsync(pv, new PublicVolumesContext()).ConfigureAwait(false);
                return vr.IsNull ? null : After(vr.Response, context);
            }

            var pp = network.PublicPriceProviders.FirstOrDefault(x => x.PricingFeatures.HasVolume && x.IsDirect);
            var pricing = pp?.PricingFeatures;

            if (pricing?.HasBulk == true && pricing.Bulk.CanVolume && pricing.Bulk.CanReturnAll)
            {
                var pr = await ApiCoordinator.GetPricingAsync(pp, new PublicPricesContext()).ConfigureAwait(false);
                return pr.IsNull ? null : After(new PublicVolumeResponse(pp.Network, pr.Response), context);
            }

            if (context?.UseReturnAll == true)
                return null;

            context.UseReturnAll = false;

            var pairs = AsyncContext.Run(() => AssetPairProvider.I.GetPairsAsync(network));

            return await GetAsync(network, pairs, context).ConfigureAwait(false);
        }

        public async Task<PublicVolumeResponse> GetAsync(Network network, IEnumerable<AssetPair> pairs, VolumeProviderContext context = null)
        {
            if (pairs.Count() > 1 && context.UseReturnAll != false)
                return await GetAsync(network, context).ConfigureAwait(false);
            
            var pv = network.PublicVolumeProviders.FirstOrDefault(x => x.IsDirect);
            var volume = pv?.VolumeFeatures;

            if (volume?.HasBulk == true)
            {
                var vr = await ApiCoordinator.GetPublicVolumeAsync(pv, new PublicVolumesContext(pairs.AsList())).ConfigureAwait(false);
                return vr.IsNull ? null : After(vr.Response, context);
            }

            var pp = network.PublicPriceProviders.FirstOrDefault(x => x.PricingFeatures.HasVolume && x.IsDirect);
            var pricing = pp?.PricingFeatures;

            if (pricing?.HasBulk == true && pricing.Bulk.CanVolume)
            {
                var pr = await ApiCoordinator.GetPricingAsync(pp, new PublicPricesContext(pairs.AsList())).ConfigureAwait(false);
                return pr.IsNull ? null : After(new PublicVolumeResponse(network, pr.Response), context);
            }

            if (context?.OnlyBulk == true)
                return null;

            var singles = await pairs.Select(x => GetAsync(network, x, context)).WhenAll().ConfigureAwait(false);

            return singles != null ? After(new PublicVolumeResponse(singles),context) : null;
        }

        public async Task<PublicVolumeResponse> GetAsync(Network network, AssetPair pair, VolumeProviderContext context = null)
        {
            var pv = network.PublicVolumeProviders.FirstOrDefault(x => x.IsDirect);

            if (pv != null)
            {
                var vr = await ApiCoordinator.GetPublicVolumeAsync(pv, new PublicVolumeContext(pair)).ConfigureAwait(false);
                return vr.IsNull ? null : After(vr.Response, context);
            }

            var pp = network.PublicPriceProviders.FirstOrDefault(x => x.PricingFeatures.HasVolume && x.IsDirect);
            var pricing = pp?.PricingFeatures;

            if (pricing?.HasVolume == true)
            {
                var pr = await ApiCoordinator.GetPricingAsync(pp, new PublicPriceContext(pair)).ConfigureAwait(false);
                return pr.IsNull ? null : After(new PublicVolumeResponse(network, pr.Response), context);
            }

            return null;
        }

        private PublicVolumeResponse After(PublicVolumeResponse response, VolumeProviderContext context)
        {
            if (response == null || context?.AfterData == null)
                return response;

            context?.AfterData?.Invoke(response);
            return response;
        }
    }
}
