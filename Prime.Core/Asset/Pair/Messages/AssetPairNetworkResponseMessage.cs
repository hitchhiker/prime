using System.Collections.Generic;
using System.Linq;

namespace Prime.Core
{
    public class AssetPairNetworkResponseMessage
    {
        public readonly IReadOnlyList<AssetPair> Pairs;
        public readonly Network Network;

        public AssetPairNetworkResponseMessage(Network network, IEnumerable<AssetPair> pairs)
        {
            Network = network;
            Pairs = pairs as IReadOnlyList<AssetPair> ?? pairs.ToList();
        }
    }
}