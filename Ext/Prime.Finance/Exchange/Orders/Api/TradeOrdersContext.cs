using Prime.Core;

namespace Prime.Finance
{
    public class TradeOrdersContext : MarketOrdersContext
    {
        public TradeOrdersContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}