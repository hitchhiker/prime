using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Common
{
    internal class AssetPairProvider
    {
        private AssetPairProvider() {}

        internal static AssetPairProvider I => Lazy.Value;
        private static readonly Lazy<AssetPairProvider> Lazy = new Lazy<AssetPairProvider>(()=>new AssetPairProvider());

        private readonly CacheDictionary<IAssetPairsProvider, AssetPairs> _cache = new CacheDictionary<IAssetPairsProvider, AssetPairs>(TimeSpan.FromHours(12));

        public async Task<IReadOnlyList<Network>> GetNetworksAsync(AssetPair pair, bool onlyDirect = false)
        {
            var provs = new ConcurrentBag<Network>();
            var q = onlyDirect ? Networks.I.AssetPairsProviders.Where(x=>x.IsDirect) : (IEnumerable<INetworkProvider>)Networks.I.AssetPairsProviders;

            var tasks = q.Select(x => x.Network).Distinct().Select(async n =>
            {
                var r = await GetPairsAsync(n);
                if (!r.Contains(pair))
                    return;

                provs.Add(n);
            });

            await Task.WhenAll(tasks);

            return provs.ToUniqueList();
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

        [Obsolete("This locking method is no good, it's blocking parallel")]
        public async Task<AssetPairs> GetPairsAsync(Network network, bool onlyDirect = true)
        {
            var q = onlyDirect ? network?.Providers.OfType<IAssetPairsProvider>().Where(x => x.IsDirect) : network?.Providers.OfType<IAssetPairsProvider>();
            var prov = q.FirstProvider();
            if (prov == null)
            {
                Logging.I.DefaultLogger.Error($"An instance of {nameof(IAssetPairsProvider)} cannot be located for {network.Name}");
                return null;
            }

            var task = new Task<AssetPairs>(() =>
            {
                return _cache.Try(prov, k =>
                {
                    var r = ApiCoordinator.GetAssetPairs(k);
                    return r.Response ?? new AssetPairs();
                });
            });

            task.Start();
            return await task;
        }
    }
}