using Prime.Utility;

namespace Prime.Common
{
    public class AggVolumeDataContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;

        public AggVolumeDataContext(AssetPair pair, ILogger logger = null) : base(logger)
        {
            Pair = pair;
        }
    }
}