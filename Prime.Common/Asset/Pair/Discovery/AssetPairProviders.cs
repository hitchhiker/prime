using System.Collections.Generic;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPairProviders
    {
        public readonly AssetPair Pair;
        public readonly IReadOnlyList<IPublicPriceProvider> Providers;
        public readonly bool IsPairReversed;

        public AssetPairProviders(AssetPair pair, IReadOnlyList<IPublicPriceProvider> providers, bool isPairReversed = false)
        {
            Pair = pair;
            Providers = providers.OrderByVolume(pair);
            Provider = providers.FirstProviderByVolume(pair);
            IsPairReversed = isPairReversed;
        }

        public AssetPair OriginalPair => IsPairReversed ? Pair.Reverse() : Pair;

        public IPublicPriceProvider Provider { get; set; }

        public bool IsPegged { get; set; }

        public bool IsIntermediary { get; set; }

        public AssetPairProviders Via { get; set; }
    }
}