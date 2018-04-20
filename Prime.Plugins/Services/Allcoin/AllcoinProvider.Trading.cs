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

        public Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var market = context.Market.ToTicker(this).ToLower();

            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "symbol", market },
                { "order_id", context.RemoteGroupId}
            };

            var rRaw = await api.GetOrderInfoAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var rOrders = rRaw.GetContent();
            
            var order = rOrders.orders?.FirstOrDefault(x=>x.order_id.Equals(context.RemoteGroupId));
            if(order == null)
                throw new NoTradeOrderException(context, this);

            var isOpen = order.status == 0 || order.status == 1; //status:  0 = unfilled, 1 = partially filled, 2 = fully filled, 10 = cancelled

            var isBuy = order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, order.order_id, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    Rate = order.price,
                    Market = order.symbol.ToAssetPair(this),
                    AmountInitial = order.amount
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(true, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
