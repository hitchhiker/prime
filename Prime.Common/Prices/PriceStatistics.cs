namespace Prime.Common
{
    public class PriceStatistics
    {
        public PriceStatistics(Asset quoteAsset, decimal? volume24, decimal? volume24Quote, decimal? lowestAsk, decimal? highestBid, decimal? price24Low, decimal? price24High)
        {
            HasVolume24 = volume24 != null;
            HasVolume24Quote = volume24Quote != null;

            Volume24 = volume24 ?? 0;
            Volume24Quote = volume24Quote ?? 0;

            HighestBid = highestBid == null ? Money.Zero : new Money(highestBid.Value, quoteAsset);
            LowestAsk = lowestAsk == null ? Money.Zero : new Money(lowestAsk.Value, quoteAsset);
            Price24Low = price24Low == null ? Money.Zero : new Money(price24Low.Value, quoteAsset);
            Price24High = price24High == null ? Money.Zero : new Money(price24High.Value, quoteAsset);
        }

        public decimal Volume24Quote { get; private set; }

        public decimal Volume24 { get; private set; }

        /// <summary>
        /// Highest 'Bid' or 'Buy Order'
        /// </summary>
        public Money HighestBid { get; private set; }

        /// <summary>
        /// Lowest 'Ask' or 'Sell Order'
        /// </summary>
        public Money LowestAsk { get; private set; }

        /// <summary>
        /// Last 24 hours price low.
        /// </summary>
        public Money Price24Low { get; private set; }

        /// <summary>
        /// Last 24 hours price high.
        /// </summary>
        public Money Price24High { get; private set; }

        public bool HasVolume24 { get; private set; }

        public bool HasVolume24Quote { get; private set; }

        public bool HasHighestBid => HighestBid != Money.Zero;

        public bool HasLowestAsk => LowestAsk != Money.Zero;

        public bool HasPrice24Low => Price24Low != Money.Zero;

        public bool HasPrice24High => Price24High != Money.Zero;
    }
}