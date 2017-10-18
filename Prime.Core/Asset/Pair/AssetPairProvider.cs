using System;
using System.Linq;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Core
{
    internal class AssetPairProvider
    {
        private AssetPairProvider() {}

        internal static AssetPairProvider I => Lazy.Value;
        private static readonly Lazy<AssetPairProvider> Lazy = new Lazy<AssetPairProvider>(()=>new AssetPairProvider());

        public async Task<AssetPairs> GetPairsAsync(Network network)
        {
            var prov = network?.Providers.OfType<IExchangeProvider>().FirstOrDefault();
            if (prov == null)
            {
                Logging.I.DefaultLogger.Warn($"No {nameof(IExchangeProvider)} found for {network}");
                return null;
            }

            var r = await ApiCoordinator.GetAssetPairsAsync(prov);
            if (r.IsNull)
                return null;

            return r.Response;
        }
    }
}