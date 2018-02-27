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
            if (r.GetContent() is BitBaySchema.ErrorBaseResponse rErrorResponse)
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

            var timestamp = (long)DateTime.UtcNow.ToUnixTimeStamp();

            var body = new Dictionary<string, object>
            {
                {"method", "trade"},
                {"moment", timestamp},
                {"type", context.IsBuy ? "buy" : "sell"},
                {"currency", context.Pair.Asset1.ShortCode},
                {"amount", context.Quantity},
                {"payment_currency", context.Pair.Asset2.ShortCode},
                {"rate", context.Rate.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.order_id);
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var timestamp = (long)DateTime.UtcNow.ToUnixTimeStamp();

            var body = new Dictionary<string, object>
            {
                { "method", "orders" },
                { "moment", timestamp}
            };

            if (!context.HasMarket)
                throw new ApiResponseException("Market should be specified when querying order status", this);

            var rOrdersRaw = await api.QueryOrdersAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rOrdersRaw);

            var order = rOrdersRaw.GetContent().FirstOrDefault(x => x.order_id.Equals(context.RemoteGroupId));

            if (order == null)
            {
                throw new NoTradeOrderException(context, this);
            }

            bool isOpen = order.status.Equals("active", StringComparison.OrdinalIgnoreCase);

            return new TradeOrderStatus(context.RemoteGroupId, isOpen, false)
            {
                Rate = order.current_price,
                AmountInitialNumeric = order.start_price
            };
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var timestamp = (long)DateTime.UtcNow.ToUnixTimeStamp();

            var body = new Dictionary<string, object>
            {
                { "method", "transfer" },
                { "moment", timestamp},
                {"currency", context.Amount.Asset.ShortCode},
                { "quantity", context.Amount.ToDecimalValue()},
                { "address", context.Address.Address}
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            // No id is returned.
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
