namespace KrakenApi
{
    public class AddOrderResult
    {
        /// <summary>
        /// Order description info.
        /// </summary>
        public AddOrderDescr Descr;

        /// <summary>
        /// Array of transaction ids for order (if order was added successfully).
        /// </summary>
        public string[] Txid;
    }
}