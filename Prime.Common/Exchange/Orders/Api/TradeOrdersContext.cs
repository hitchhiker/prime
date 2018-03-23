using Prime.Utility;

namespace Prime.Common
{
    public class TradeOrdersContext : NetworkProviderPrivateContext
    {
        public TradeOrdersContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}