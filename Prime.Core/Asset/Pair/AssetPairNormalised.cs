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
            if (string.CompareOrdinal(originalPair.Asset1.ShortCode, originalPair.Asset2.ShortCode)>0)
            {
                Normalised = new AssetPair(originalPair.Asset2, originalPair.Asset1);
                IsNormalised = true;
            }
            else
                Normalised = originalPair;
        }
    }
}