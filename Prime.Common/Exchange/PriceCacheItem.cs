using System;
using Prime.Utility;

namespace Prime.Common
{
    public class PriceCacheItem
    {
        public AssetPair Pair { get; set; }

        public Money Price { get; set; }

        public bool IsMissing { get; set; }

        public DateTime UtcEntered { get; set; }

        public IPublicPairPriceProvider Provider { get; set; }

        public bool Match(IPublicPairPriceProvider provider, AssetPair pair)
        {
            if (provider.Id != Provider.Id)
                return false;

            return UtcEntered.IsWithinTheLast(TimeSpan.FromMinutes(-1)) && pair.Equals(Pair);
        }
    }
}