namespace CryptoFacilities.Api.V1
{
    public class OrderInfo
    {
        /// <summary>
        /// The identifier of the order
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// The submission time of the order
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// The currency of denomination of the contract (always USD)
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The name of the contract (e.g. F-XBT:USD-Mar15)
        /// </summary>
        public string Tradeable { get; set; }

        /// <summary>
        /// The direction of the order, either Buy or Sell
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// The order quantity that has not been matched yet
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// The order quantity that has been matched
        /// </summary>
        public int Filled { get; set; }

        /// <summary>
        /// The order type (always LMT)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The limit buy or sell price
        /// </summary>
        public decimal Lmt { get; set; }
    }
}