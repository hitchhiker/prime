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

        [Bson]
        public Asset QuoteAsset { get; set; }

        [Bson]
        public Money Price { get; set; }

        public LatestPrice Reverse()
        {
            return new LatestPrice(new Money(1d / Price, QuoteAsset), Price.Asset);
        }

        public override string ToString()
        {
            return $"{QuoteAsset}: {Price.Display}";
        }
    }
}