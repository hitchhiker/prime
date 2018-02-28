using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Gemini
{
    public partial class GeminiProvider : IOrderLimitProvider
    {
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;
        public Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            throw new NotImplementedException();
        }

        public Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public MinimumTradeVolume[] MinimumTradeVolume { get; }
        public OrderLimitFeatures OrderLimitFeatures { get; }
    }
}
