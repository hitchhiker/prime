namespace KrakenApi
{
    public class OrderBook
    {
        /// <summary>
        /// Ask side array of array entries(<price>, <volume>, <timestamp>)
        /// </summary>
        public decimal[][] Asks;

        /// <summary>
        /// Bid side array of array entries(<price>, <volume>, <timestamp>)
        /// </summary>
        public decimal[][] Bids;
    }
}