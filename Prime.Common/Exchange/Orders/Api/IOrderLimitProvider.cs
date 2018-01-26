using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IOrderLimitProvider : INetworkProviderPrivate
    {
        Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context);

        Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context);

        MinimumTradeVolume[] MinimumTradeVolume { get; }
    }
}