using Prime.Utility;

namespace Prime.Common
{
    public class TradeOrdersContext : NetworkProviderPrivateContext
    {
        public TradeOrdersContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
        
        public AssetPair Market { get; set; } = AssetPair.Empty;
        public bool HasMarket => Market != null && !Market.Equals(AssetPair.Empty);
    }
}