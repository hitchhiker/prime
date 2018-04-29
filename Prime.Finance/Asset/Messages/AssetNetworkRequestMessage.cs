using Prime.Core;

namespace Prime.Finance
{
    public class AssetNetworkRequestMessage
    {
        public readonly Network Network;

        public AssetNetworkRequestMessage(Network network)
        {
            Network = network;
        }
    }
}