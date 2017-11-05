using System;

namespace Prime.Common
{
    internal class AssetPairCache : ProviderCache<IAssetPairsProvider, ApiResponse<AssetPairs>>
    {
        public AssetPairCache() : base(TimeSpan.FromHours(12)) { }

        public static AssetPairCache I => Lazy.Value;
        private static readonly Lazy<AssetPairCache> Lazy = new Lazy<AssetPairCache>(()=>new AssetPairCache());
    }
}