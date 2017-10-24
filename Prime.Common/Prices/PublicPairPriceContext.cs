using Prime.Utility;

namespace Prime.Common
{
    public class PublicPairPriceContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;

        public PublicPairPriceContext(AssetPair pair, ILogger logger = null) : base(logger)
        {
            Pair = pair;
        }
    }
}