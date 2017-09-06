using Prime.Utility;

namespace Prime.Core
{
    public class PublicPriceContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;

        public PublicPriceContext(AssetPair pair)
        {
            Pair = pair;
        }

        public PublicPriceContext(AssetPair pair, Logger logger) : base(logger)
        {
            Pair = pair;
        }
    }
}