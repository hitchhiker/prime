using Prime.Utility;

namespace Prime.Common
{
    public class PrivatePairContext : NetworkProviderPrivateContext
    {
        public readonly AssetPair Pair;

        public PrivatePairContext(UserContext userContext, AssetPair pair, ILogger logger) : base(userContext, logger)
        {
            Pair = pair;
        }
    }
}