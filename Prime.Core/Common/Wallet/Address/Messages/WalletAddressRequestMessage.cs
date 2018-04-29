namespace Prime.Core
{
    public class WalletAddressRequestMessage
    {
        public readonly Asset Asset;
        public readonly Network Network;

        public WalletAddressRequestMessage(Network network, Asset asset)
        {
            Network = network;
            Asset = asset;
        }
    }
}