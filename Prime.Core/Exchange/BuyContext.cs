using Prime.Utility;

namespace Prime.Core
{
    public class BuyContext : NetworkProviderPrivateContext {

        public BuyContext(UserContext userContext, ILogger logger) : base(userContext, logger)
        {
        }
    }
}