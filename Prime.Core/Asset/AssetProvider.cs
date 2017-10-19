using System;
using System.Linq;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Core
{
    internal class AssetProvider
    {
        private AssetProvider() {}

        internal static AssetProvider I => Lazy.Value;
        private static readonly Lazy<AssetProvider> Lazy = new Lazy<AssetProvider>(()=>new AssetProvider());

        public async Task<UniqueList<Asset>> GetAssetsAsync(Network network)
        {
            if (network == null)
                return null;

            var pairs = await AssetPairProvider.I.GetPairsAsync(network);
            if (pairs == null)
                return null;

            var assets = pairs.Select(x => x.Asset1).Union(pairs.Select(x => x.Asset2)).OrderBy(x => x.ShortCode);

            return assets.ToUniqueList();
        }
    }
}