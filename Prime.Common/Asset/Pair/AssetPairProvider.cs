using System;
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

        public IReadOnlyList<IAssetPairsProvider> GetProvidersFromPrivate(AssetPair pair)
        {
           return AsyncContext.Run(() => AssetPairProvider.I.GetProvidersFromPrivateAsync(pair));
        }

        public async Task<IReadOnlyList<IAssetPairsProvider>> GetProvidersFromPrivateAsync(AssetPair pair)
        {
            var provs = new List<IAssetPairsProvider>();
            foreach (var prov in Networks.I.AssetPairsProviders.WithApi())
            {
                var r = await GetPairsAsync(prov.Network);
                if (r.Contains(pair))
                    provs.Add(prov);
            }
            return provs;
        }

        public async Task<AssetPairs> GetPairsAsync(Network network)
        {
            var prov = network?.Providers.OfType<IExchangeProvider>().FirstOrDefault();
            if (prov == null)
            {
                Logging.I.DefaultLogger.Error($"An instance of {nameof(IAssetPairsProvider)} cannot be located for {network.Name}");
                return null;
            }

            var task = new Task<AssetPairs>(() =>
            {
                return _cache.Try(prov, k =>
                {
                    var r = AsyncContext.Run(() => ApiCoordinator.GetAssetPairsAsync(k));
                    return r.Response;
                });
            });

            task.Start();
            return await task;
        }
    }
}