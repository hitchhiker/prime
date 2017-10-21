using Prime.Utility;

namespace Prime.Common
{
    public class WalletAddressAssetContext : WalletAddressContext
    {
        public Asset Asset { get; set; }

        public WalletAddressAssetContext(Asset asset, bool canGenerateAddress, UserContext userContext, ILogger logger = null) : base(canGenerateAddress, userContext, logger)
        {
            Asset = asset;
        }
    }
}