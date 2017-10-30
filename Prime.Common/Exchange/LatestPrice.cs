using System;

namespace Prime.Common
{
    public class LatestPrice
    {
        public LatestPrice() { }

        public LatestPrice(AssetPair pair, decimal value) : this(new Money(value, pair.Asset1), pair.Asset2)
        {
        }

        public LatestPrice(Money price, Asset quoteAsset)
        {
            UtcCreated = DateTime.UtcNow;
            Price = price;
            QuoteAsset = quoteAsset;
        }

        [Bson]
        public DateTime UtcCreated { get; set; }

        [Obsolete("Maybe should be called BaseAsset")]
        [Bson]
        public Asset QuoteAsset { get; set; } // BUG: rename to base asset?

        [Bson]
        public Money Price { get; set; }

        public override string ToString()
        {
            return $"{QuoteAsset}: {Price.Display}";
        }
    }
}