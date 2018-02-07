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

            if (r.success == false)
            {
                throw new ApiResponseException("Unsuccessful request in PlaceOrderLimitAsync");
            }

            return new PlacedOrderLimitResponse(r.returnData.order_id);
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            if (!context.HasMarket)
                throw new ApiResponseException("Market should be specified when querying order status", this);

            var bodyActiveOrders = new Dictionary<string, object>
            {
                { "pair", context.Market.ToTicker(this)}
            };

            var bodyOrderInfo = new Dictionary<string, object>
            {
                { "order_id", context.RemoteGroupId}
            };

            //Checks if this order is contained in active list.
            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync(bodyActiveOrders).ConfigureAwait(false);
            CheckResponseErrors(rActiveOrdersRaw);
            
            //If the active list contains this order and the request for active orders was successful, then it is active.  Otherwise it is not active.
            bool isOpen = rActiveOrdersRaw.GetContent().success && rActiveOrdersRaw.GetContent().returnData.ContainsKey(context.RemoteGroupId);

            var rOrderRaw = await api.QueryOrderInfoAsync(bodyOrderInfo).ConfigureAwait(false);
            CheckResponseErrors(rOrderRaw);

            var order = rOrderRaw.GetContent();
            
            if (order == null || order.success == false)
            {
                throw new NoTradeOrderException(context, this);
            }
            
            return new TradeOrderStatus(context.RemoteGroupId, isOpen, false)
            {
                Rate = order.returnData.Value.rate,
                AmountInitial = new Money(order.returnData.Value.start_amount, context.Market.Asset2)
            };
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = new Dictionary<string, object>
            {
                {"coinName", context.Amount.Asset.ShortCode},
                { "amount", context.Amount.ToDecimalValue()},
                { "address", context.Address.Address}
            };
            
            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            if (rRaw.GetContent().success == false)
            {
                throw new ApiResponseException("Unsuccessful request in PlaceWithdrawalAsync");
            }

            // No id is returned.
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
