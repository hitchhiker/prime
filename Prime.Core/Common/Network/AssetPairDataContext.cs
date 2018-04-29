using Prime.Core;

namespace Prime.Core
{
    public class AssetPairDataContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;

        public readonly AggregatedAssetPairData Document;

        public AssetPairDataContext(AggregatedAssetPairData data, ILogger logger = null) : base(logger)
        {
            Pair = data.AssetPair;
            Document = data;
        }
    }
}