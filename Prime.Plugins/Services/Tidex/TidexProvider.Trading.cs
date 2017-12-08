using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;

namespace Prime.Plugins.Services.Tidex
{
    public partial class TidexProvider : IOrderLimitProvider
    {
        public Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreateTidexPostBody();
            body.Add("method", "orderInfo");
            body.Add("order_id", context.RemoteId);

            var r = await api.GetOrderInfoAsync(body).ConfigureAwait(false);

            CheckTidexResponse(r);

            if(r.return_.Count == 0 || !r.return_.TryGetValue(context.RemoteId, out var order))
                throw new NoTradeOrderException(context, this);

            return new TradeOrderStatus(context.RemoteId, order.status == 0, order.status == 2 || order.status == 3);
        }

        public decimal MinimumTradeVolume { get; }
    }
}
