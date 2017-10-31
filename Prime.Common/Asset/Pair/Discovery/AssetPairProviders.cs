using System.Collections.Generic;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPairProviders
    {
        public readonly AssetPair Pair;
        public readonly IReadOnlyList<IPublicPriceSuper> Providers;
        public readonly bool IsPairReversed;

        public AssetPairProviders(AssetPair pair, IReadOnlyList<IPublicPriceSuper> providers, bool isPairReversed = false)
        {
            Pair = pair;
            Providers = providers.OrderByVolume(pair);
            Provider = providers.FirstProviderByVolume(pair);
            IsPairReversed = isPairReversed;
        }

        public AssetPair OriginalPair => IsPairReversed ? Pair.Reverse() : Pair;

        public IPublicPriceSuper Provider { get; set; }

        public bool IsPegged { get; set; }

        public bool IsIntermediary { get; set; }

        public AssetPairProviders Via { get; set; }
    }
}