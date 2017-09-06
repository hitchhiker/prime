using Prime.Utility;

namespace Prime.Core
{
    public class AggregatedCoinInfoContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;

        public AggregatedCoinInfoContext(AssetPair pair, Logger logger = null) : base(logger)
        {
            Pair = pair;
        }
    }
}