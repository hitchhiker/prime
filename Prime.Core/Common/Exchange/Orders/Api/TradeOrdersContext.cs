using Prime.Core;

namespace Prime.Core
{
    public class TradeOrdersContext : MarketOrdersContext
    {
        public TradeOrdersContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}