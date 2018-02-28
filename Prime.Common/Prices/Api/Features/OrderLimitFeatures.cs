using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class OrderLimitFeatures
    {
        public OrderLimitFeatures(bool requiresMarketForOrderStatus, bool canGetMarketByOrder)
        {
            RequiresMarketForOrderStatus = requiresMarketForOrderStatus;
            CanGetMarketByOrder = canGetMarketByOrder;
        }

        /// <summary>
        /// Indicates whether market is required when order status is queried.
        /// </summary>
        public bool RequiresMarketForOrderStatus { get; }

        /// <summary>
        /// Indicates whether provider can get market of order with specified id.
        /// </summary>
        public bool CanGetMarketByOrder { get; }

        /// <summary>
        /// Indicates whether GetMarketByOrder request affects rate limiter by more that 1.
        /// In Binance the number of requests counted against the rate limiter is equal to the number of symbols currently trading on the exchange.
        /// </summary>
        [Obsolete("Should be revised and probably removed.")]
        public bool MarketByOrderRequstAffectsRateLimiter { get; set; }
    }
}
