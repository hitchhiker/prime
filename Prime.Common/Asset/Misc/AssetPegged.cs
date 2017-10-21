using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    internal class AssetPegged
    {
        public static List<(string a1, string a2)> Pegged = new List<(string, string)> { ("USD", "USDT") };

        public static UniqueList<Asset> GetPegged(Asset asset)
        {
            var r = new UniqueList<Asset>();

            var k = Pegged.Where(x => x.a1 == asset.ShortCode).Select(x => x.a2.ToAssetRaw());
            r.AddRange(k);

            var v = Pegged.Where(x => x.a2 == asset.ShortCode).Select(x => x.a1.ToAssetRaw());
            r.AddRange(v);

            r.RemoveAll(x => x.ShortCode == asset.ShortCode);

            return r;
        }
    }
}