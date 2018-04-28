using System;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Common;

namespace Prime.Core
{
    internal class AssetProvider
    {
        private AssetProvider() {}

        internal static AssetProvider I => Lazy.Value;
        private static readonly Lazy<AssetProvider> Lazy = new Lazy<AssetProvider>(()=>new AssetProvider());

        private readonly CacheDictionary<Network, UniqueList<Asset>> _cache = new CacheDictionary<Network, UniqueList<Asset>>(TimeSpan.FromHours(12));

        public async Task<UniqueList<Asset>> GetAssetsAsync(Network network)
        {
            if (network == null)
                return null;

            var task = new Task<UniqueList<Asset>>(() =>
            {
                return _cache.Try(network, k =>
                {
                    var pairs = AsyncContext.Run(() => AssetPairProvider.I.GetPairsAsync(network));
                    var assets = pairs?.Select(x => x.Asset1).Union(pairs.Select(x => x.Asset2)).OrderBy(x => x.ShortCode);
                    return assets?.ToUniqueList();
                });
            });

            task.Start();
            return await task.ConfigureAwait(false);
        }

        public async Task<UniqueList<Asset>> GetAllAsync(bool onlyDirect = false)
        {
            var r = await AssetPairProvider.I.GetPairsAsync(onlyDirect).ConfigureAwait(false);
            return r.Select(x => x.Asset1).Union(r.Select(x => x.Asset2)).ToUniqueList();
        }
    }
}