using System;

namespace Prime.Common
{
    public class MarketPrice
    {
        private MarketPrice() { }

        public MarketPrice(AssetPair pair, decimal value, DateTime? utcCreated = null)
        {
            Pair = pair;
            UtcCreated = utcCreated ?? DateTime.UtcNow;
            Price = new Money(value, pair.Asset1);
        }

        public MarketPrice(Money price, Asset quoteAsset) : this(new AssetPair(price.Asset, quoteAsset), price)
        {
        }

        public Asset QuoteAsset => Pair.Asset2;

        [Bson]
        public AssetPair Pair { get; private set; }

        [Bson]
        public DateTime UtcCreated { get; private set; }

        [Bson]
        public Money Price { get; private set; }

        public MarketPrice Reverse()
        {
            return new MarketPrice(Price.ReverseAsset(QuoteAsset), Price.Asset);
        }

        public override string ToString()
        {
            return $"1 {QuoteAsset} = {Price.Display}";
        }
    }
}