namespace Prime.Common
{
    public class AssetPairDiscoveryResultMessage
    {
        public readonly AssetPairProviders Providers;
        public readonly AssetPairDiscoveryRequestMessage RequestRequestMessage;
        public readonly bool IsFailed;

        public AssetPairDiscoveryResultMessage(AssetPairDiscoveryRequestMessage requestRequest, AssetPairProviders providers)
        {
            RequestRequestMessage = requestRequest;
            Providers = providers;
            IsFailed = Providers == null || Providers.Providers.Count == 0;
        }

        public AssetPairDiscoveryResultMessage(AssetPairDiscoveryRequestMessage requestRequest)
        {
            RequestRequestMessage = requestRequest;
            IsFailed = true;
        }
    }
}