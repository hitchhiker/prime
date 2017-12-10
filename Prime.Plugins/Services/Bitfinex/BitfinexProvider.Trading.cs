using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Bitfinex
{
    public partial class BitfinexProvider : IOrderLimitProvider
    {

        public Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public decimal MinimumTradeVolume { get; }
    }
}
