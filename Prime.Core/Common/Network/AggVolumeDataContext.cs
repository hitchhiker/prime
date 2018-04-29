using Prime.Core;

namespace Prime.Core
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