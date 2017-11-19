namespace Prime.Common
{
    public class PriceStatistics
    {
        private PriceStatistics() { }

        public PriceStatistics(Network network, Asset quoteAsset, decimal? lowestAsk = null, decimal? highestBid = null, decimal? price24Low = null, decimal? price24High = null)
        {
            Network = network;

            HighestBid = highestBid == null ? Money.Zero : new Money(highestBid.Value, quoteAsset);
            LowestAsk = lowestAsk == null ? Money.Zero : new Money(lowestAsk.Value, quoteAsset);
            Price24Low = price24Low == null ? Money.Zero : new Money(price24Low.Value, quoteAsset);
            Price24High = price24High == null ? Money.Zero : new Money(price24High.Value, quoteAsset);
        }

        [Bson]
        public Network Network { get; private set; }

        /// <summary>
        /// Highest 'Bid' or 'Buy Order'
        /// </summary>
        [Bson]
        public Money HighestBid { get; private set; }

        /// <summary>
        /// Lowest 'Ask' or 'Sell Order'
        /// </summary>
        [Bson]
        public Money LowestAsk { get; private set; }

        /// <summary>
        /// Last 24 hours price low.
        /// </summary>
        [Bson]
        public Money Price24Low { get; private set; }

        /// <summary>
        /// Last 24 hours price high.
        /// </summary>
        [Bson]
        public Money Price24High { get; private set; }

        public bool HasHighestBid => HighestBid != Money.Zero;

        public bool HasLowestAsk => LowestAsk != Money.Zero;

        public bool HasPrice24Low => Price24Low != Money.Zero;

        public bool HasPrice24High => Price24High != Money.Zero;
    }
}