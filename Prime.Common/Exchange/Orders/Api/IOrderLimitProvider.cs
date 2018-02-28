using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IOrderLimitProvider : INetworkProviderPrivate
    {
        Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context);

        /// <summary>
        /// Gets the status of order with specific 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context);

        /// <summary>
        /// Gets the market where order with specified Remote Id is placed.
        /// </summary>
        /// <param name="context">Remote Id of order.</param>
        /// <returns>Market where specified order was placed.</returns>
        Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context); 

        MinimumTradeVolume[] MinimumTradeVolume { get; }

        OrderLimitFeatures OrderLimitFeatures { get; }
    }
}