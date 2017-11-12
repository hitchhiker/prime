using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Common;

namespace Prime.Plugins.Services
{
    internal static class AssetsUtilities
    {
        public static (String AssetCode1, String AssetCode2)? GetAssetPair(string pairCode, AssetPairs existingPairs)
        {
            if (pairCode.Length == 6)
            {
                return GetAssetPairByLength(pairCode);
            }
            else if (pairCode.Length > 6)
            {
                var existingAsset = FindAssetByPairCode(pairCode, existingPairs);

                if (existingAsset == null)
                    return null;

                var asset1 = pairCode.Replace(existingAsset.ShortCode, "");
                var asset2 = existingAsset.ShortCode;

                if (pairCode.StartsWith(existingAsset.ShortCode))
                    return (asset2, asset1);
                else
                    return (asset1, asset2);
            }

            return null;
        }

        private static Asset FindAssetByPairCode(string pairCode, AssetPairs pairs)
        {
            var asset = pairs.FirstOrDefault(x => pairCode.StartsWith(x.Asset1.ShortCode))?.Asset1;
            if (asset != null)
                return asset;

            asset = pairs.FirstOrDefault(x => pairCode.StartsWith(x.Asset2.ShortCode))?.Asset2;
            if (asset != null)
                return asset;

            asset = pairs.FirstOrDefault(x => pairCode.EndsWith(x.Asset1.ShortCode))?.Asset1;
            if (asset != null)
                return asset;

            asset = pairs.FirstOrDefault(x => pairCode.EndsWith(x.Asset2.ShortCode))?.Asset2;
            if (asset != null)
                return asset;

            return null;
        }

        public static (string asset1, string asset2)? GetAssetPairByLength(string rawCode)
        {
            if (rawCode.Length != 6)
                return null;

            return (rawCode.Substring(0, 3), rawCode.Substring(3));
        }
    }
}
