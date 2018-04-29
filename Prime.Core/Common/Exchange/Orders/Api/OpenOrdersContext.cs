using Prime.Core;

namespace Prime.Core
{
    public class OpenOrdersContext : MarketOrdersContext
    {
        public OpenOrdersContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}