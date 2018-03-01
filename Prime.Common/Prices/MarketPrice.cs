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

        /// <summary>
        /// Do not use this constructor unless you are absolutely sure of the order.
        /// </summary>
        /// <param name="network"></param>
        /// <param name="quoteAsset"></param>
        /// <param name="price"></param>
        public MarketPrice(Network network, Asset quoteAsset, Money price) : this(network, new AssetPair(quoteAsset, price.Asset), price)
        {
        }

        [Bson]
        public PriceStatistics PriceStatistics { get; set; }
        
        [Bson]
        public NetworkPairVolume Volume { get; set; }

        public Asset QuoteAsset => Pair.Asset1;

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public AssetPair Pair { get; private set; }

        [Bson]
        public DateTime UtcCreated { get; private set; }

        [Bson]
        public Money Price { get; private set; }

        private MarketPrice _reversed;
        public MarketPrice Reversed
        {
            get
            {
                if (_reversed == null)
                {
                    _reversed = new MarketPrice(Network, Price.Asset, Price.ReverseAsset(QuoteAsset))
                    {
                        UtcCreated = UtcCreated,
                        _reversed = this,
                        PriceStatistics = PriceStatistics?.Reverse(QuoteAsset),
                        Volume = Volume // TODO: AY: confirm Volume = Volume, maybe some transformations needed.
                    };
                }

                return _reversed;
            }
        }

        public MarketPrice AsQuote(Asset quote)
        {
            if (Equals(Pair.Asset1, quote))
                return this;
            if (Equals(Pair.Asset2, quote))
                return Reversed;
            return null;
        }

        public bool HasStatistics => PriceStatistics != null;

        public bool HasVolume => Volume != null;

        public override string ToString()
        {
            return $"{Network?.Name}: 1 {QuoteAsset} = {Price.Display}";
        }

        public decimal PercentageDifference(MarketPrice secondPrice)
        {
            if (secondPrice.Reversed.Pair.Id == this.Pair.Id)
                secondPrice = secondPrice.Reversed;

            if (secondPrice.Pair.Id != Pair.Id)
                throw new Exception($"Can't calculate percentage difference for {nameof(MarketPrice)}, as pairs don't match: {secondPrice.Pair} - {Pair}");

            return Price.PercentageProfit(secondPrice.Price);
        }
    }
}