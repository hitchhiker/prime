using System.Collections.Generic;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPairKnownProviders
    {
        public readonly AssetPair Pair;
        public readonly IReadOnlyList<IOhlcProvider> Providers;
        public readonly bool IsReversed;

        public AssetPairKnownProviders(AssetPair pair, IReadOnlyList<IOhlcProvider> providers, bool isReversed = false)
        {
            Pair = pair;
            Providers = providers;
            Provider = providers.FirstProviderByVolume(pair);
            IsReversed = isReversed;
        }

        public IOhlcProvider Provider { get; set; }

        public bool IsPegged { get; set; }

        public bool IsIntermediary { get; set; }

        public AssetPairKnownProviders Via { get; set; }
    }
}