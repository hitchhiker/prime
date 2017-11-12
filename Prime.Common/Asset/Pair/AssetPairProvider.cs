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
            var r  = await GetNetworkPairsAsync().ConfigureAwait(false);
            return r.Select(x => x.Key).ToUniqueList();
        }

        public async Task<IReadOnlyDictionary<Network, AssetPairs>> GetNetworksDiskAsync()
        {
            var results = new ConcurrentDictionary<Network, AssetPairs>();
            var q = Networks.I.AssetPairsProviders.Where(x => x.IsDirect);
            var nets = q.Select(x => x.Network).ToUniqueList();
            foreach (var n in nets)
            {
                var d = PublicContext.I.As<AssetPairData>().FirstOrDefault(x => x.Id == AssetPairData.GetHash(n));
                if (d != null)
                    results.Add(n, d.Pairs);
            }

            var remain = nets.Where(x => !results.ContainsKey(x)).ToUniqueList();

            if (!remain.Any())
                return results;

            foreach (var kv in await GetNetworkPairsAsync(remain).ConfigureAwait(false))
            {
                results.Add(kv.Key, kv.Value);
                var ad = new AssetPairData(kv.Key, kv.Value);
                ad.SavePublic();
            }

            return results;
        }

        public Task<IReadOnlyDictionary<Network,AssetPairs>> GetNetworkPairsAsync(bool onlyDirect = true)
        {
            var q = onlyDirect ? Networks.I.AssetPairsProviders.Where(x => x.IsDirect) : (IEnumerable<INetworkProvider>)Networks.I.AssetPairsProviders;
            return GetNetworkPairsAsync(q.Select(x => x.Network).Distinct().ToUniqueList(), onlyDirect);
        }

        public async Task<IReadOnlyDictionary<Network, AssetPairs>> GetNetworkPairsAsync(UniqueList<Network> networks, bool onlyDirect = true)
        {
            var results = new ConcurrentDictionary<Network, AssetPairs>();

            var tasks = networks.Select(async n =>
            {
                var r = await GetPairsAsync(n).ConfigureAwait(false);
                results.TryAdd(n, r);
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);
            return results.ToDictionary(x => x.Key, y => y.Value);
        }

        public async Task<IReadOnlyList<AssetPair>> GetPairsAsync(bool onlyDirect = true)
        {
            var pairs = new ConcurrentBag<AssetPair>();
            var q = onlyDirect ? Networks.I.AssetPairsProviders.Where(x => x.IsDirect) : Networks.I.AssetPairsProviders;
            var tasks = q.Select(async prov =>
            {
                var r = await GetPairsAsync(prov.Network).ConfigureAwait(false);
                if (r == null)
                    return;

                foreach (var p in r)
                    pairs.Add(p);
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);

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

            var r = await ApiCoordinator.GetAssetPairsAsync(prov).ConfigureAwait(false);
            return r.IsNull ? null : r.Response;
        }
    }
}