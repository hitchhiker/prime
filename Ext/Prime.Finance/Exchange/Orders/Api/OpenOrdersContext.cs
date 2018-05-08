using Prime.Core;

namespace Prime.Finance
{
    public class OpenOrdersContext : MarketOrdersContext
    {
        public OpenOrdersContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}