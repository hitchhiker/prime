using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class OrderLimitFeatures
    {
        public OrderLimitFeatures(bool requiresMarketForOrderStatus, CanGetOrderMarket canGetOrderMarket)
        {
            RequiresMarketForOrderStatus = requiresMarketForOrderStatus;
            CanGetOrderMarket = canGetOrderMarket;
        }

        /// <summary>
        /// Indicates whether market is required when order status is queried.
        /// </summary>
        public bool RequiresMarketForOrderStatus { get; }

        public CanGetOrderMarket CanGetOrderMarket { get; }

        /// <summary>
        /// Indicates whether provider can get order market with specified id by calling specific API endpoint.
        /// </summary>
        public bool CanGetMarketByAdditionalRequest => CanGetOrderMarket == CanGetOrderMarket.ByAdditionalRequest;

        /// <summary>
        /// Indicates whether provider can return market of order when it's status queried by id.
        /// </summary>
        public bool CanGetMarketWithinOrderStatus => CanGetOrderMarket == CanGetOrderMarket.WithinOrderStatus;

        /// <summary>
        /// Indicates whether GetMarketByOrder request affects rate limiter by more that 1.
        /// In Binance the number of requests counted against the rate limiter is equal to the number of symbols currently trading on the exchange.
        /// </summary>
        [Obsolete("Should be revised and probably removed.")]
        public bool MarketByOrderRequstAffectsRateLimiter { get; set; }
    }
}
