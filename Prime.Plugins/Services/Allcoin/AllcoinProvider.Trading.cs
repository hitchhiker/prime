using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.Allcoin
{
    public partial class AllcoinProvider : IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.TryGetContent(out AllcoinSchema.ErrorResponse rError))
                if (!rError.result)
                    throw new ApiResponseException(rError.error_code, this, method);
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "symbol", context.Pair.ToTicker(this).ToLower() },
                { "type", context.IsBuy ? "buy" : "sell"},
                { "amount", context.Quantity.ToDecimalValue()},
                { "price", context.Rate.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);
            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.order_id);
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            string market = context.Market.ToTicker(this).ToLower();

            var order = await GetOrderReponseByIdAsync(context, market).ConfigureAwait(false);

            var isOpen = order.status == 0 || order.status == 1; //status:  0 = unfilled, 1 = partially filled, 2 = fully filled, 10 = cancelled

            var isBuy = order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatus(order.order_id, isBuy, isOpen, false)
            {
                Rate = order.price,
                Market = market
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        private async Task<AllcoinSchema.OrderInfoEntryResponse> GetOrderReponseByIdAsync(RemoteIdContext context, string market)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "symbol", market.ToLower() },
                { "order_id", context.RemoteGroupId}
            };

            var rRaw = await api.GetOrderInfoAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var rOrders = rRaw.GetContent();

            if (rOrders.orders != null && rOrders.orders.Length > 0)
            {
                var order = rOrders.orders.FirstOrDefault(x=>x.order_id.Equals(context.RemoteGroupId));

                if (order == null)
                {
                    throw new NoTradeOrderException(context.RemoteGroupId, this);
                }

                return order;
            }
            else
            {
                throw new NoTradeOrderException(context.RemoteGroupId, this);
            }
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.FromNowhere);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
