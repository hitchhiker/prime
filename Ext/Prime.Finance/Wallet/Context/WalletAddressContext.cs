using Prime.Core;

namespace Prime.Finance
{
    public class WalletAddressContext : NetworkProviderPrivateContext
    {
        public WalletAddressContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
            
        }
    }
}