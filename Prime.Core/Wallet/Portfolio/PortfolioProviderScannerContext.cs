using Prime.Utility;

namespace Prime.Core.Wallet
{
    public class PortfolioProviderScannerContext
    {
        public PortfolioProviderScannerContext(UserContext context, IWalletService provider, Asset baseAsset, int frequency = 15000)
        {
            Context = context;
            Provider = provider;
            BaseAsset = baseAsset;
            Frequency = frequency;
        }

        public UserContext Context { get; set; }
        public IWalletService Provider { get; set; }
        public Asset BaseAsset { get; set; }
        public int Frequency { get; set; }
        public Logger L { get; set; } = Logging.I.Common;
    }
}