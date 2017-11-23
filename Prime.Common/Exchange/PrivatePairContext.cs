using Prime.Utility;

namespace Prime.Common
{
    public class PrivatePairContext : NetworkProviderPrivateContext
    {
        public readonly AssetPair Pair;

        public PrivatePairContext(UserContext userContext, AssetPair pair = null, ILogger logger = null) : base(userContext, logger)
        {
            Pair = pair;
        }

        public bool HasPair => Pair != null;

        public string RemotePairOrNull(IDescribesAssets provider)
        {
            return HasPair ?  Pair.ToTicker(provider) : null;
        }
    }
}