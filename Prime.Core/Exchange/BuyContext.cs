using Prime.Utility;

namespace Prime.Core
{
    public class BuyContext : NetworkProviderPrivateContext {

        public BuyContext(UserContext userContext, Logger logger) : base(userContext, logger)
        {
        }
    }
}