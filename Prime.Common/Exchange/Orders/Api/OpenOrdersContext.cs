using Prime.Utility;

namespace Prime.Common
{
    public class OpenOrdersContext : MarketOrdersContext
    {
        public OpenOrdersContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}