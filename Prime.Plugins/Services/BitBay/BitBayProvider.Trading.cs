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

namespace Prime.Plugins.Services.BitBay
{
    public partial class BitBayProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            var rErrorResponse = r.GetContent() as BitBaySchema.ErrorBaseResponse;
            if (rErrorResponse != null)
            {
                if (!string.IsNullOrEmpty(rErrorResponse.message))
                    throw new ApiResponseException($"{rErrorResponse.code}: {rErrorResponse.message}", this, method);
            }

            if (!r.ResponseMessage.IsSuccessStatusCode)
                throw new ApiResponseException($"{r.ResponseMessage.ReasonPhrase} ({r.ResponseMessage.StatusCode})",
                    this, method);
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreatePostBody("trade");
            body.Add("type", context.IsBuy ? "buy" : "sell");
            body.Add("currency", context.Pair.Asset1.ToRemoteCode(this));
            body.Add("amount", context.Quantity);
            body.Add("payment_currency", context.Pair.Asset2.ToRemoteCode(this));
            body.Add("rate", context.Rate.ToDecimalValue());

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.order_id);
        }

        private async Task<BitBaySchema.OrdersResponse> GetOrderResponseByOrderId(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreatePostBody("orders");

            var rRaw = await api.QueryOrdersAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();
            var order = r.FirstOrDefault(x => x.order_id.Equals(context.RemoteGroupId));

            if (order == null)
                throw new NoTradeOrderException(context, this);

            return order;
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var order = await GetOrderResponseByOrderId(context).ConfigureAwait(false);
            var isOpen = order.status.Equals("active", StringComparison.OrdinalIgnoreCase);

            var isBuy = order.type.Equals("bid", StringComparison.OrdinalIgnoreCase);

            return new TradeOrderStatus(context.RemoteGroupId, isBuy, isOpen, false)
            {
                Rate = order.current_price,
                AmountInitial = order.start_price
            };
        }

        public async Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            var order = await GetOrderResponseByOrderId(context).ConfigureAwait(false);

            // TODO: check if market is returned correctly - BitBay.
            return new OrderMarketResponse(new AssetPair(order.order_currency, order.payment_currency, this));
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var timestamp = (long)DateTime.UtcNow.ToUnixTimeStamp();

            var body = new Dictionary<string, object>
            {
                {"method", "transfer"},
                {"moment", timestamp},
                {"currency", context.Amount.Asset.ShortCode},
                {"quantity", context.Amount.ToDecimalValue()},
                {"address", context.Address.Address}
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            // No id is returned from exchange.
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, true);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
