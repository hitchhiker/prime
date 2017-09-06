using Prime.Utility;

namespace Prime.Core
{
    public class SellContext : NetworkProviderPrivateContext {

        public SellContext(UserContext userContext, Logger logger = null) : base(userContext, logger)
        {
        }
    }
}