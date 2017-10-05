using Prime.Utility;

namespace Prime.Core
{
    public class AssetPairDataContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;

        public readonly AssetPairData Document;

        public AssetPairDataContext(AssetPairData data, ILogger logger = null) : base(logger)
        {
            Pair = data.AssetPair;
            Document = data;
        }
    }
}