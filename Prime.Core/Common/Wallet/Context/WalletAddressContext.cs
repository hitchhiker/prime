using Prime.Core;

namespace Prime.Core
{
    public class WalletAddressContext : NetworkProviderPrivateContext
    {
        public WalletAddressContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
            
        }
    }
}