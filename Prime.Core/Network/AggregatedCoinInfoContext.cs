using Prime.Utility;

namespace Prime.Core
{
    public class AggregatedCoinInfoContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;

        public AggregatedCoinInfoContext(AssetPair pair, ILogger logger = null) : base(logger)
        {
            Pair = pair;
        }
    }
}