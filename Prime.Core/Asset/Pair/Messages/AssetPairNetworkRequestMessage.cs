namespace Prime.Core
{
    public class AssetPairNetworkRequestMessage
    {
        public readonly Network Network;

        public AssetPairNetworkRequestMessage(Network network)
        {
            Network = network;
        }
    }
}