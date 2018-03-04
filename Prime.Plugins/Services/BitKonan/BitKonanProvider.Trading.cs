using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.BitKonan
{
    public partial class BitKonanProvider : IOrderLimitProvider
    {
        //TODO: This method still needs to be tested with a balance greater than 0 in the account, to see what it returns (documentation does not explain). It should return the order ID.
        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "pair", context.Pair.ToTicker(this,'/').ToUpper() },
                { "side", context.IsBuy ? "BUY" : "SELL"},
                { "type", "LIMIT"},
                { "amount", context.Quantity.ToDecimalValue()},
                { "limit", context.Rate.ToDecimalValue()}
            };

            var r = await api.NewOrderAsync(body).ConfigureAwait(false);

            return r?.status.Equals("success", StringComparison.OrdinalIgnoreCase) == true ? new PlacedOrderLimitResponse(r.data?.order_id) : new PlacedOrderLimitResponse("");
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var order = await GetOrderReponseByIdAsync(context).ConfigureAwait(false);

            // Checks if this order is contained in active list.
            var activeOrders = await api.QueryActiveOrdersAsync().ConfigureAwait(false);

            if (activeOrders.status.Equals("success", StringComparison.OrdinalIgnoreCase) == false)
            {
                throw new ApiResponseException(activeOrders.errors != null && activeOrders.errors.Length > 0 ? activeOrders.errors[0] : "Error in QueryActiveOrdersAsync");
            }

            // If the active list contains this order and the request for active orders was successful, then it is open. Otherwise it is not open.
            var isOpen = activeOrders.data.Any(x => x.id.Equals(context.RemoteGroupId));

            var isBuy = order.type.IndexOf("BUY", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatus(order.id, isBuy, isOpen, false)
            {
                Rate = order.price,
                AmountInitial = order.amount,
                Market = order.trade_pair
            };
        }

        private async Task<BitKonanSchema.OrderInfoEntryResponse> GetOrderReponseByIdAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.QueryActiveOrdersAsync().ConfigureAwait(false);

            if (r.status.Equals("success", StringComparison.OrdinalIgnoreCase) == false)
            {
                throw new ApiResponseException(r.errors != null && r.errors.Length > 0 ? r.errors[0] : "Error in GetOrderReponseByIdAsync");
            }

            var order = r.data.FirstOrDefault(x => x.id.Equals(context.RemoteGroupId));

            if (order == null)
            {
                throw new NoTradeOrderException(context.RemoteGroupId, this);
            }

            return order;
        }

        public async Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            var order = await GetOrderReponseByIdAsync(context).ConfigureAwait(false);

            var orderMarket = order.trade_pair.ToAssetPair(this, '/');

            return new OrderMarketResponse(orderMarket);
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;
    }
}
