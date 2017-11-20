using System.Collections.Generic;
using Prime.Common;

namespace Prime.Common
{
    public class NetworkContext
    {
        public NetworkContext()
        {
            Meta = new GraphMeta(this);
        }

        public NetworkContext(GraphMeta meta)
        {
            Meta = meta;
        }

        public GraphMeta Meta { get; }
        public List<Asset> BadCoins { get; set; } = new List<Asset>();
        public List<Network> BadNetworks { get; set; } = new List<Network>();
        public List<AssetPair> OnlyPairs { get; set; } = new List<AssetPair>();
        public List<Network> OnlyNetworks { get; set; } = new List<Network>();
    }
}