using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public class NetworksForPair
    {
        public readonly AssetPair Pair;
        public IReadOnlyList<Network> Networks { get; private set; }
        public IReadOnlyList<NetworkApiPair> ForApi { get; private set; }

        public NetworksForPair(IEnumerable<Network> networks, AssetPair pair)
        {
            Pair = pair;
            Networks = networks.AsReadOnlyList();
        }

        public void PrepareApi(NetworkGraph graph)
        {
            ForApi = Networks.Select(network => new NetworkApiPair(network, Pair, graph)).ToList();
        }

        public void Trim()
        {
            var bad = ForApi.Where(x => x.Pair == null).ToList();
            if (!bad.Any())
                return;

            ForApi = ForApi.Where(x => x.Pair != null).ToList();
            Networks = ForApi.Select(x => x.Network).ToList();
        }
    }
}