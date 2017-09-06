namespace KrakenApi
{
    public class CancelOrderResult
    {
        /// <summary>
        /// Number of orders canceled.
        /// </summary>
        public int Count;

        /// <summary>
        /// If set, order(s) is/are pending cancellation.
        /// </summary>
        public bool? Pending;
    }
}