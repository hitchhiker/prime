using Prime.Core;

namespace Prime.Finance
{
    public class WalletAddressAssetContext : WalletAddressContext
    {
        public Asset Asset { get; set; }

        public WalletAddressAssetContext(Asset asset, UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
            Asset = asset;
        }
    }
}