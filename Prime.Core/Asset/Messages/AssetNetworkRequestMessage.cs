namespace Prime.Core
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