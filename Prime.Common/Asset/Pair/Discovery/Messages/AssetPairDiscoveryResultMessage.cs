namespace Prime.Common
{
    public class AssetPairDiscoveryResultMessage
    {
        public readonly AssetPairNetworks Networks;
        public readonly AssetPairDiscoveryRequestMessage RequestRequestMessage;
        public readonly bool IsFailed;

        public AssetPairDiscoveryResultMessage(AssetPairDiscoveryRequestMessage requestRequest, AssetPairNetworks networks)
        {
            RequestRequestMessage = requestRequest;
            Networks = networks;
            IsFailed = Networks == null || Networks.Providers.Count == 0;
        }

        public AssetPairDiscoveryResultMessage(AssetPairDiscoveryRequestMessage requestRequest)
        {
            RequestRequestMessage = requestRequest;
            IsFailed = true;
        }
    }
}