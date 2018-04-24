using System.Collections.Concurrent;

namespace Prime.Common
{
    public class NetworkApiPair
    {
        public readonly Network Network;
        public readonly AssetPair Pair;
        private readonly ConcurrentDictionary<Network, INetworkProvider> _providers = new ConcurrentDictionary<Network, INetworkProvider>();

        public NetworkApiPair(Network network, AssetPair pair, NetworkGraph graph)
        {
            Network = network;
            Pair = graph.GetOriginalPair(network, pair);
        }

        public T GetProvider<T>() where T : class, INetworkProvider
        {
            return _providers.GetOrAdd(Network, n => n.GetProv<T>() as INetworkProvider) as T;
        }
    }
}