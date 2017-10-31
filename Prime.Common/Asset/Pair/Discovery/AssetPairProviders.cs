using System.Collections.Generic;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPairProviders
    {
        public readonly AssetPair Pair;
        public readonly IReadOnlyList<IPublicPrice> Providers;
        public readonly bool IsPairReversed;

        public AssetPairProviders(AssetPair pair, IReadOnlyList<IPublicPrice> providers, bool isPairReversed = false)
        {
            Pair = pair;
            Providers = providers.OrderByVolume(pair);
            Provider = providers.FirstProviderByVolume(pair);
            IsPairReversed = isPairReversed;
        }

        public AssetPair OriginalPair => IsPairReversed ? Pair.Reverse() : Pair;

        public IPublicPrice Provider { get; set; }

        public bool IsPegged { get; set; }

        public bool IsIntermediary { get; set; }

        public AssetPairProviders Via { get; set; }
    }
}