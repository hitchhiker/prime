using Prime.Utility;

namespace Prime.Common
{
    public class RemoteIdContext : NetworkProviderPrivateContext
    {
        public readonly string RemoteId;

        public RemoteIdContext(UserContext userContext, string remoteId, ILogger logger = null) : base(userContext, logger)
        {
            RemoteId = remoteId;
        }

        /// <summary>
        /// Required for some exchanges that don't support direct status querying (Cryptopia).
        /// </summary>
        public AssetPair Market = AssetPair.Empty;
        public bool HasMarket => !Equals(Market, AssetPair.Empty);
    }
}