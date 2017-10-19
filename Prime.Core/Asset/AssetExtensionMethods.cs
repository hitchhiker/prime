using System;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Core
{
    public static class AssetExtensionMethods
    {
        public static AssetPair ToAssetPair(this string pair, IDescribesAssets provider, char delimiter = '_')
        {
            if (string.IsNullOrWhiteSpace(pair) || pair.IndexOf(delimiter) == -1)
                return AssetPair.Empty;

            var p = pair.Split(delimiter);
            return p.Length != 2 ? AssetPair.Empty : new AssetPair(p[0], p[1], provider);
        }

        /// <summary>
        /// Returns the assetCode as an Asset object, no remote provider conversion of codes is done. 
        /// Use with caution.
        /// </summary>
        /// <param name="assetCode"></param>
        /// <returns></returns>
        public static Asset ToAssetRaw(this string assetCode)
        {
            return Assets.I.GetRaw(assetCode);
        }

        public static Asset ToAsset(this string assetCode, IDescribesAssets provider)
        {
            if (provider == null)
                throw new ArgumentException(nameof(provider));

            var cv = provider.GetAssetCodeConverter();
            if (cv == null)
                return Assets.I.Get(assetCode, provider);

            return Assets.I.Get(cv.ToLocalCode(assetCode), provider);
        }

        public static string ToRemoteCode(this Asset asset, IDescribesAssets provider)
        {
            if (provider == null)
                throw new ArgumentException(nameof(provider));

            var cv = provider.GetAssetCodeConverter();
            if (cv == null)
                return asset.ShortCode;

            return ToRemoteCode(asset, cv);
        }

        public static string ToRemoteCode(this Asset asset, IAssetCodeConverter converter)
        {
            if (converter == null)
                throw new ArgumentException(nameof(converter));

            return converter.ToRemoteCode(asset);
        }

        public static bool IsNone(this Asset asset)
        {
            return asset == null || Equals(asset, Asset.None);
        }

        public static Asset EqualOrPegged(this IEnumerable<Asset> items, Asset asset)
        {
            foreach (var i in items)
            {
                if (i.Equals(asset))
                    return i;
                if (asset.Pegged.Contains(i))
                    return i;
            }
            return null;
        }
    }
}