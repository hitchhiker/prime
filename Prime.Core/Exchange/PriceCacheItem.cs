using System;
using Prime.Utility;

namespace Prime.Core
{
    public class PriceCacheItem
    {
        public AssetPair Pair { get; set; }

        public Money Price { get; set; }

        public DateTime UtcEntered { get; set; }

        public bool Match(AssetPair pair)
        {
            return UtcEntered.IsWithinTheLast(TimeSpan.FromMinutes(-1)) && pair.Equals(Pair);
        }
    }
}