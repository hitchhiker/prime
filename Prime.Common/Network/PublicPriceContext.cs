using Prime.Utility;

namespace Prime.Common
{
    public class PublicPriceContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;

        public PublicPriceContext(AssetPair pair, ILogger logger = null) : base(logger)
        {
            Pair = pair;
        }
    }
}