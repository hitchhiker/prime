using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Common
{
    internal class AssetPairProvider
    {
        private AssetPairProvider() {}

        internal static AssetPairProvider I => Lazy.Value;
        private static readonly Lazy<AssetPairProvider> Lazy = new Lazy<AssetPairProvider>(()=>new AssetPairProvider());

        public async Task<IReadOnlyList<Network>> GetNetworksAsync(AssetPair pair, bool onlyDirect = false)
        {
            var networks = new ConcurrentBag<Network>();
            var q = onlyDirect ? Networks.I.AssetPairsProviders.Where(x=>x.IsDirect) : (IEnumerable<INetworkProvider>)Networks.I.AssetPairsProviders;

            var tasks = q.Select(x => x.Network).Distinct().Select(async n =>
            {
                var r = await GetPairsAsync(n);
                if (!r.Contains(pair))
                    return;

                networks.Add(n);
            });

            await Task.WhenAll(tasks);

            return networks.ToUniqueList();
        }

        public async Task<IReadOnlyList<AssetPair>> GetPairsAsync(bool onlyDirect = true)
        {
            var pairs = new ConcurrentBag<AssetPair>();
            var q = onlyDirect ? Networks.I.AssetPairsProviders.Where(x => x.IsDirect) : Networks.I.AssetPairsProviders;
            var tasks = q.Select(async prov =>
            {
                var r = await GetPairsAsync(prov.Network);
                if (r == null)
                    return;

                foreach (var p in r)
                    pairs.Add(p);
            });

            await Task.WhenAll(tasks);

            return pairs.ToUniqueList();
        }

        public async Task<AssetPairs> GetPairsAsync(Network network, bool onlyDirect = true)
        {
            var q = onlyDirect ? network?.Providers.OfType<IAssetPairsProvider>().Where(x => x.IsDirect) : network?.Providers.OfType<IAssetPairsProvider>();
            var prov = q.FirstProvider();
            if (prov == null)
            {
                Logging.I.DefaultLogger.Error($"An instance of {nameof(IAssetPairsProvider)} cannot be located for {network.Name}");
                return null;
            }

            var r = await ApiCoordinator.GetAssetPairsAsync(prov);
            return r.IsNull ? null : r.Response;
        }
    }
}