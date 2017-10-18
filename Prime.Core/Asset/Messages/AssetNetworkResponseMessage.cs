using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetNetworkResponseMessage
    {
        public readonly Network Network;
        public readonly IReadOnlyList<Asset> Assets;

        public AssetNetworkResponseMessage(Network network, UniqueList<Asset> assets)
        {
            Network = network;
            Assets = assets;
        }
    }
}