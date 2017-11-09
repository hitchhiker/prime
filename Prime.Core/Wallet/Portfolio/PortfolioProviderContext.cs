using Prime.Utility;

namespace Prime.Common.Wallet
{
    public class PortfolioProviderContext
    {
        public PortfolioProviderContext(UserContext context, IBalanceProvider provider, Asset baseAsset, int frequency = 15000)
        {
            Context = context;
            Provider = provider;
            BaseAsset = baseAsset;
            Frequency = frequency;
        }

        public UserContext Context { get; set; }
        public IBalanceProvider Provider { get; set; }
        public Asset BaseAsset { get; set; }
        public int Frequency { get; set; }
        public ILogger L { get; set; } = Logging.I.DefaultLogger;
    }
}