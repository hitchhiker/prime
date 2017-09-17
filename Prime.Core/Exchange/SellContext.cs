using Prime.Utility;

namespace Prime.Core
{
    public class SellContext : NetworkProviderPrivateContext {

        public SellContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}