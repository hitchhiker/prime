using System;
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

        private readonly CacheDictionary<IExchangeProvider, AssetPairs> _cache = new CacheDictionary<IExchangeProvider, AssetPairs>(TimeSpan.FromHours(12));

        public async Task<AssetPairs> GetPairsAsync(Network network)
        {
            var prov = network?.Providers.OfType<IExchangeProvider>().FirstOrDefault();
            if (prov == null)
            {
                Logging.I.DefaultLogger.Warn($"No {nameof(IExchangeProvider)} found for {network}");
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