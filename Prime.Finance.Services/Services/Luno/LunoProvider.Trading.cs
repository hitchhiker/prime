using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.Luno
{
    public partial class LunoProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (rawResponse.TryGetContent(out LunoSchema.ErrorResponse baseResponse))
            {
                if (!string.IsNullOrWhiteSpace(baseResponse.error_code))
                    throw new ApiResponseException($"Error Code: {baseResponse.error_code} - {baseResponse.error.TrimEnd('.')}", this, method);
            }

            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                var reason = rawResponse.ResponseMessage.ReasonPhrase;

                throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var side = context.IsBuy ? "BID" : "ASK";

            var body = new Dictionary<string, object>
            {
                { "volume", context.Quantity.ToDecimalValue()},
                { "price", context.Rate.ToDecimalValue()},
                { "pair", context.Pair.ToTicker(this).ToUpper()},
                { "type", side}
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

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var orderRaw = await api.QueryOrderAsync(context.RemoteGroupId).ConfigureAwait(false);

            CheckResponseErrors(orderRaw);

            var order = orderRaw.GetContent();

            var isBuy = order.type.IndexOf("BID", StringComparison.OrdinalIgnoreCase) >= 0;
            var isOpen = order.state.IndexOf("PENDING", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, order.order_id, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    AmountInitial = order.limit_volume,
                    Rate = order.limit_price,
                    Market = context.Market
                }
            };
        }

        public Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "amount", context.Amount.ToDecimalValue()},
                { "currency", context.Amount.Asset.ShortCode.ToUpper()},
                { "address", context.Address}
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.success ? new WithdrawalPlacementResult() : null;
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.FromNowhere);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
