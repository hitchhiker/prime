using Prime.Utility;

namespace Prime.Core
{
    public class NetworkProviderPrivateContext : NetworkProviderContext
    {
        public readonly UserContext UserContext;

        public NetworkProviderPrivateContext(UserContext userContext, ILogger logger = null) : base(logger)
        {
            UserContext = userContext;
        }

        public override bool IsPublic => UserContext == null;

        public virtual ApiKey GetKey(INetworkProvider provider)
        {
            return !IsPrivate ? null : UserContext.GetApiKey(provider);
        }
    }
}