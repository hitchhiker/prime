using Prime.Common;

namespace Prime.Common
{
    /// <summary>
    /// Base context which has optional field Market. Successors of this class used to query order history or open orders.
    /// </summary>
    public class MarketOrdersContext : NetworkProviderPrivateContext
    {
        public MarketOrdersContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
        
        public AssetPair Market { get; set; } = AssetPair.Empty;
        public bool HasMarket => Market != null && !Market.Equals(AssetPair.Empty);
    }
}