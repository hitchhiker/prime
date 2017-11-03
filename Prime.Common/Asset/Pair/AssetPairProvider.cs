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

        public IReadOnlyList<Network> GetNetworks(AssetPair pair, bool onlyPrivate = false)
        {
           return AsyncContext.Run(() => GetNetworksAsync(pair, onlyPrivate));
        }

        public async Task<IReadOnlyList<Network>> GetNetworksAsync(AssetPair pair, bool onlyPrivate = false)
        {
            var provs = new List<Network>();
            var q = onlyPrivate ? Networks.I.AssetPairsProviders.WithApi() : (IEnumerable<INetworkProvider>)Networks.I.AssetPairsProviders;
            foreach (var n in q.Select(x=>x.Network).Distinct())
            {
                var r = await GetPairsAsync(n);
                if (r.Contains(pair))
                    provs.Add(n);
            }
            return provs;
        }

        public IReadOnlyList<AssetPair> GetPairs(bool onlyPrivate = false)
        {
            return AsyncContext.Run(() => GetPairs(onlyPrivate));
        }

        public async Task<IReadOnlyList<AssetPair>> GetPairsAsync(bool onlyPrivate = false)
        {
            var pairs = new UniqueList<AssetPair>();
            var q = onlyPrivate ? Networks.I.AssetPairsProviders.WithApi() : (IEnumerable<IAssetPairsProvider>)Networks.I.AssetPairsProviders;
            foreach (var prov in q)
            {
                var r = await GetPairsAsync(prov.Network);
                if (r!=null)
                    pairs.AddRange(r);
            }
            return pairs;
        }

        public async Task<AssetPairs> GetPairsAsync(Network network)
        {
            var prov = network?.Providers.OfType<IAssetPairsProvider>().FirstProvider();
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