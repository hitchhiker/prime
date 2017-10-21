using Prime.Utility;

namespace Prime.Common
{
    public class BuyContext : NetworkProviderPrivateContext {

        public BuyContext(UserContext userContext, ILogger logger) : base(userContext, logger)
        {
        }
    }
}