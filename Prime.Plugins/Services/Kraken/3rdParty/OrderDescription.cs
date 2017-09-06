namespace KrakenApi
{
    public class OrderDescription
    {
        /// <summary>
        /// Asset pair.
        /// </summary>
        public string Pair;

        /// <summary>
        /// Type of order (buy/sell).
        /// </summary>
        public string Type;

        /// <summary>
        /// Order type (See Add standard order).
        /// </summary>
        public string OrderType;

        /// <summary>
        /// Primary price.
        /// </summary>
        public decimal Price;

        /// <summary>
        /// Secondary price
        /// </summary>
        public decimal Price2;

        /// <summary>
        /// Amount of leverage
        /// </summary>
        public string Leverage;

        /// <summary>
        /// Order description.
        /// </summary>
        public string Order;

        /// <summary>
        /// Conditional close order description (if conditional close set).
        /// </summary>
        public string Close;
    }
}