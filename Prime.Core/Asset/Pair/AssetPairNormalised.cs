using System;

namespace Prime.Core.Exchange.Rates
{
    public class AssetPairNormalised
    {
        public readonly AssetPair OriginalPair;
        public readonly AssetPair Normalised;
        public readonly bool IsNormalised;

        public AssetPairNormalised(AssetPair originalPair)
        {
            OriginalPair = originalPair;
            Normalised = originalPair.Normalised;
            IsNormalised = originalPair.IsNormalised;
        }
    }
}