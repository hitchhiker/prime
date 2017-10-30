namespace Prime.Common
{
    public class AssetPairDiscoveryResultMessage
    {
        public readonly AssetPairProviders Providers;
        public readonly AssetPairDiscoveryRequestMessage RequestRequestMessage;

        public AssetPairDiscoveryResultMessage(AssetPairDiscoveryRequestMessage requestRequest, AssetPairProviders providers)
        {
            RequestRequestMessage = requestRequest;
            Providers = providers;
        }
    }
}