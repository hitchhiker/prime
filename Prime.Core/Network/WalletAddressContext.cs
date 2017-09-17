using Prime.Utility;

namespace Prime.Core
{
    public class WalletAddressContext : NetworkProviderPrivateContext
    {
        public readonly bool CanGenerateAddress;

        public WalletAddressContext(bool canGenerateAddress, UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
            CanGenerateAddress = canGenerateAddress;
        }
    }
}