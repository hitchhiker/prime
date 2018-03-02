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

namespace Prime.Plugins.Services.Yobit
{
    public partial class YobitProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (!r.ResponseMessage.IsSuccessStatusCode)
                throw new ApiResponseException($"{r.ResponseMessage.ReasonPhrase} ({r.ResponseMessage.StatusCode})",
                    this, method);

            if (r.GetContent() is YobitSchema.BaseResponse<T> baseResponse)
            {
                if (!baseResponse.success)
                    throw new ApiResponseException("API response error occurred", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "pair", context.Pair.ToTicker(this) },
                { "type", context.IsBuy ? "buy" : "sell"},
                { "amount", context.Quantity},
                { "rate", context.Rate.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.returnData.order_id);
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var order = await GetOrderReponseByIdAsync(context).ConfigureAwait(false);

            var bodyActiveOrders = new Dictionary<string, object>
            {
                {"pair", order.pair}
            };

            // Checks if this order is contained in active list.
            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync(bodyActiveOrders).ConfigureAwait(false);
            CheckResponseErrors(rActiveOrdersRaw);

            var activeOrders = rActiveOrdersRaw.GetContent().returnData;
            // If the active list contains this order and the request for active orders was successful, then it is active. Otherwise it is not active.
            var isOpen = activeOrders.ContainsKey(context.RemoteGroupId);

            var isBuy = order.type.Equals("buy", StringComparison.OrdinalIgnoreCase);

            return new TradeOrderStatus(context.RemoteGroupId, isBuy, isOpen, false)
            {
                Market = order.pair.ToAssetPair(this),
                Rate = order.rate,
                AmountInitial = order.start_amount
            };
        }

        private async Task<YobitSchema.OrderInfoEntryResponse> GetOrderReponseByIdAsync(RemoteIdContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var bodyOrderInfo = new Dictionary<string, object>
            {
                { "order_id", context.RemoteGroupId}
            };

            var rOrderRaw = await api.QueryOrderInfoAsync(bodyOrderInfo).ConfigureAwait(false);
            CheckResponseErrors(rOrderRaw);

            var orderContent = rOrderRaw.GetContent();
            if (!orderContent.returnData.Key.Equals(context.RemoteGroupId))
                throw new NoTradeOrderException(context.RemoteGroupId, this);

            var order = orderContent.returnData.Value;

            return order;
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = new Dictionary<string, object>
            {
                {"coinName", context.Amount.Asset.ShortCode},
                {"amount", context.Amount.ToDecimalValue()},
                {"address", context.Address.Address}
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            // No id is returned from exchange.
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
