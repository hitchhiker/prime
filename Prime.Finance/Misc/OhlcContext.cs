using Prime.Core;

namespace Prime.Finance
{
    public class OhlcContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;
        public readonly TimeResolution Resolution;
        public readonly TimeRange Range;

        public OhlcContext(AssetPair pair, TimeResolution resolution, TimeRange range, ILogger logger = null) : base(logger)
        {
            Pair = pair;
            Resolution = resolution;
            Range = range;
        }
    }
}