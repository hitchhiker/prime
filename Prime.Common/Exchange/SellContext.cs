using Prime.Utility;

namespace Prime.Common
{
    public class SellContext : NetworkProviderPrivateContext {

        public SellContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}