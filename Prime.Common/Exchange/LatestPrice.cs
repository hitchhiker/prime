using System;

namespace Prime.Common
{
    public class LatestPrice
    {
        public LatestPrice() { }

        public LatestPrice(AssetPair pair, decimal value)
        {
            Price = new Money(value, pair.Asset1);
            BaseAsset = pair.Asset2;
        }

        public LatestPrice(Money price)
        {
            UtcCreated = DateTime.UtcNow;
            Price = price;
        }

        [Bson]
        public DateTime UtcCreated { get; set; }

        [Bson]
        public Asset BaseAsset { get; set; }

        [Bson]
        public Money Price { get; set; }
    }
}