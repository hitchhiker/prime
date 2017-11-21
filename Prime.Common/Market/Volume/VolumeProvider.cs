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
            var t = GetTask(network, context);
            if (t == null)
                return null;
            return await t.ConfigureAwait(false);
        }

        public async Task<PublicVolumeResponse> GetAsync(Network network, IEnumerable<AssetPair> pairs, VolumeProviderContext context = null)
        {
            var t = GetTask(network, pairs, context);
            if (t == null)
                return null;
            return await t.ConfigureAwait(false);
        }

        public async Task<PublicVolumeResponse> GetAsync(VolumeProviderContext context = null)
        {
            var t = GetTask(context);
            if (t == null)
                return null;
            return await t.ConfigureAwait(false);
        }

        public async Task<PublicVolumeResponse> GetAsync(IEnumerable<Network> networks, VolumeProviderContext context = null)
        {
            var t = GetTask(networks, context);
            if (t == null)
                return null;
            return await t.ConfigureAwait(false);
        }

        public async Task<PublicVolumeResponse> GetAsync(Network network, AssetPair pair,VolumeProviderContext context = null)
        {
            var t = GetTask(network, pair, context);
            if (t == null)
                return null;
            return await t.ConfigureAwait(false);
        }

        public Task<PublicVolumeResponse> GetTask(IEnumerable<Network> networks, VolumeProviderContext context = null)
        {
            var nets = networks.ToList();
            var tasks = nets.Select(x => GetAsync(x, context)).Where(x => x != null);
            return tasks.WhenAll().ContinueWith(r => After(new PublicVolumeResponse(r.Result), context));
        }

        public Task<PublicVolumeResponse> GetTask(Network network, VolumeProviderContext context = null)
        {
            var pv = network.PublicVolumeProviders.FirstOrDefault(x => x.IsDirect);
            var volume = pv?.VolumeFeatures;

            if (volume?.HasBulk == true && volume.Bulk.CanReturnAll)
                return CoordinatorTask(pv, new PublicVolumesContext());

            var pp = PricingProvider.I.GetTask(network, new PricingProviderContext() { OnlyBulk = true, RequestVolume = true, UseReturnAll = true });
            if (pp != null)
                return pp.ContinueWith(r => After(new PublicVolumeResponse(network, r.Result), context));

            if (context?.UseReturnAll == true)
                return default;

            var newContext = context?.Clone() ?? new VolumeProviderContext();
            newContext.UseReturnAll = false;

            var pairs = AsyncContext.Run(() => AssetPairProvider.I.GetPairsAsync(network));
            if (pairs == null)
                return default;

            return GetTask(network, pairs, newContext);
        }

        public Task<PublicVolumeResponse> GetTask(Network network, IEnumerable<AssetPair> pairs, VolumeProviderContext context = null)
        {
            if (pairs == null)
                return default;

            if (pairs.Count() > 1 && context?.UseReturnAll != false)
                return GetTask(network, context);
            
            var pv = network.PublicVolumeProviders.FirstOrDefault(x => x.IsDirect);
            var volume = pv?.VolumeFeatures;

            if (volume?.HasBulk == true)
                return CoordinatorTask(pv, new PublicVolumesContext(pairs.AsList()));
            
            var pp = PricingProvider.I.GetTask(network, pairs, new PricingProviderContext() {OnlyBulk = true, RequestVolume = true});
            if (pp != null)
                return pp.ContinueWith(r => After(r.Result == null ? null : new PublicVolumeResponse(network, r.Result), context));
            
            if (context?.OnlyBulk == true)
                return default;

            var singles = pairs.Select(x => GetAsync(network, x, context)).Where(x => x != null).WhenAll();
            return singles.ContinueWith(r => After(r.Result == null ? null : new PublicVolumeResponse(r.Result), context));
        }

        public Task<PublicVolumeResponse> GetTask(VolumeProviderContext context = null)
        {
            var nets = Networks.I.ToList();
            return GetAsync(nets, context);
        }

        public Task<PublicVolumeResponse> GetTask(Network network, AssetPair pair, VolumeProviderContext context = null)
        {
            var pv = network.PublicVolumeProviders.FirstOrDefault(x => x.IsDirect);
            if (pv != null)
                return CoordinatorTask(pv, new PublicVolumeContext(pair));
            
            var pp = PricingProvider.I.GetTask(network, pair, new PricingProviderContext() { RequestVolume = true });
            if (pp != null)
                return pp.ContinueWith(r => After(r.Result == null ? null : new PublicVolumeResponse(network, r.Result), context));
            
            if (ProviderAggVolumeData != null && ProviderAggVolumeData.NetworksSupported.Contains(network))
                return CoordinatorTask(new AggVolumeDataContext(pair), context);
            
            return default;
        }

        private Task<PublicVolumeResponse> CoordinatorTask(AggVolumeDataContext vContext, VolumeProviderContext context = null)
        {
            return ApiCoordinator.GetAggVolumeDataAsync(ProviderAggVolumeData, vContext).ContinueWith(x =>
            {
                var r = x.Result;
                return r.IsNull ? null : After(r.Response, context);
            });
        }

        private Task<PublicVolumeResponse> CoordinatorTask(IPublicVolumeProvider prov, PublicVolumesContext vContext, VolumeProviderContext context = null)
        {
            return ApiCoordinator.GetPublicVolumeAsync(prov, vContext).ContinueWith(x =>
            {
                var r = x.Result;
                return r.IsNull ? null : After(r.Response, context);
            });
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
