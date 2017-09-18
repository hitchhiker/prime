using System;

namespace Prime.Core
{
    public class LatestPrice
    {
        public LatestPrice() { }

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