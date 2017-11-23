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
    }
}