using Prime.Utility;

namespace Prime.Common
{
    public class WalletAddressAssetContext : WalletAddressContext
    {
        public readonly bool CanGenerateAddress;

        public Asset Asset { get; set; }

        public WalletAddressAssetContext(Asset asset, bool canGenerateAddress, UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
            CanGenerateAddress = canGenerateAddress;
            Asset = asset;
        }
    }
}