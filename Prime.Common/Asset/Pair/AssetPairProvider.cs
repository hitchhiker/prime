using System;
using System.Collections;
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

        public IReadOnlyList<Network> GetNetworks(AssetPair pair, bool onlyDirect = false)
        {
           return AsyncContext.Run(() => GetNetworksAsync(pair, onlyDirect));
        }

        public async Task<IReadOnlyList<Network>> GetNetworksAsync(AssetPair pair, bool onlyDirect = false)
        {
            var provs = new List<Network>();
            var q = onlyDirect ? Networks.I.AssetPairsProviders.Where(x=>x.IsDirect) : (IEnumerable<INetworkProvider>)Networks.I.AssetPairsProviders;
            foreach (var n in q.Select(x=>x.Network).Distinct())
            {
                var r = await GetPairsAsync(n);
                if (r.Contains(pair))
                    provs.Add(n);
            }
            return provs;
        }

        public IReadOnlyList<AssetPair> GetPairs(bool onlyDirect = true)
        {
            return AsyncContext.Run(() => GetPairs(onlyDirect));
        }

        public async Task<IReadOnlyList<AssetPair>> GetPairsAsync(bool onlyDirect = true)
        {
            var pairs = new UniqueList<AssetPair>();
            var q = onlyDirect ? Networks.I.AssetPairsProviders.Where(x => x.IsDirect) : Networks.I.AssetPairsProviders;
            foreach (var prov in q)
            {
                var r = await GetPairsAsync(prov.Network);
                if (r!=null)
                    pairs.AddRange(r);
            }
            return pairs;
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