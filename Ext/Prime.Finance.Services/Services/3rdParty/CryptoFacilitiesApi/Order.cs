namespace CryptoFacilities.Api.V1
{
    public class Order
    {
        #region Required

        // The following parameters must be set to be able to place an order.
        // Type
        // Tradeable
        // Unit
        // Dir
        // Qty
        // Price

        /// <summary>
        /// The order type (always LMT )
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The name of the contract (e.g. F-XBT:USD-Mar15 )
        /// </summary>
        public string Tradeable { get; set; }

        /// <summary>
        /// The currency of denomination of the contract (always USD )
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The direction of the order, either Buy or Sell
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// The quantity of the order. This must be an integer number
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// The limit buy or sell price. This must be a multiple of the tick size, which is 0.01
        /// </summary>
        public decimal Price { get; set; }

        #endregion Required

        // The following parameters will be set on order execution

        /// <summary>
        /// The order identifier if the order has been placed successfully
        /// </summary>
        public string Uid { get; set; }
    }
}