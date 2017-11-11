using System;

namespace Prime.Common
{
    public class MarketPrice
    {
        private MarketPrice() { }

        /// <summary>
        /// As an example if the pair requested was BTC:USD - the value quoted should therefore be a USD value.
        /// The result would be a market price in USD, and a QuoteAsset of BTC
        /// Avoid using this constructor unless you are clear with this information.
        /// </summary>
        /// <param name="network"></param>
        /// <param name="pair"></param>
        /// <param name="value"></param>
        /// <param name="utcCreated"></param>
        public MarketPrice(Network network, AssetPair pair, decimal value, DateTime? utcCreated = null)
        {
            Network = network;
            Pair = pair;
            UtcCreated = utcCreated ?? DateTime.UtcNow;
            Price = new Money(value, pair.Asset2);
        }

        public MarketPrice(Network network, Asset quoteAsset, Money price) : this(network, new AssetPair(quoteAsset, price.Asset), price)
        {
        }

        [Bson]
        public PriceStatistics PriceStatistics { get; set; }

        public Asset QuoteAsset => Pair.Asset1;

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public AssetPair Pair { get; private set; }

        [Bson]
        public DateTime UtcCreated { get; private set; }

        [Bson]
        public Money Price { get; private set; }

        public MarketPrice Reverse()
        {
            return new MarketPrice(Network, Price.Asset, Price.ReverseAsset(QuoteAsset));
        }

        public MarketPrice AsQuote(Asset quote)
        {
            if (Equals(Pair.Asset1, quote))
                return this;
            if (Equals(Pair.Asset2, quote))
                return Reverse();
            return null;
        }

        public bool HasStatistics => PriceStatistics != null;

        public override string ToString()
        {
            return $"1 {QuoteAsset} = {Price.Display}";
        }
    }
}