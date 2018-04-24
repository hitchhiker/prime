using System.Collections.Generic;
using System.Linq;

namespace Prime.Common
{
    public class AssetPairAllResponseMessage
    {
        public readonly IReadOnlyList<AssetPair> Pairs;

        public AssetPairAllResponseMessage(IEnumerable<AssetPair> pairs)
        {
            Pairs = pairs as IReadOnlyList<AssetPair> ?? pairs.ToList();
        }
    }
}