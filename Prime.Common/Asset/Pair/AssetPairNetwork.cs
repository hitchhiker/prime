using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPairNetwork
    {
        public readonly AssetPair Pair;
        public IReadOnlyList<Network> Networks { get; }

        public AssetPairNetwork(Network network, AssetPair pair)
        {
            Pair = pair;
            Networks = new List<Network>() {network};
        }

        public AssetPairNetwork(IEnumerable<Network> networks, AssetPair pair)
        {
            Pair = pair;
            Networks = networks.AsReadOnlyList();
        }
    }
}