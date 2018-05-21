using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.LakeBtc
{
    public partial class LakeBtcProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            var reason = rawResponse.ResponseMessage.ReasonPhrase;

            if (rawResponse.TryGetContent(out LakeBtcSchema.ErrorResponse errorResponse))
            {
                if (!string.IsNullOrWhiteSpace(errorResponse?.error))
                    throw new ApiResponseException($"{errorResponse?.error.TrimEnd('.')}", this, method);
            }
            else
            {
                throw new ApiResponseException(
                    $"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}",
                    this, method);
            }
        }
        //TODO - SC - Not working (invalid symbol error, most likely due to commas in param field)
        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var method = context.IsBuy ? "buyOrder" : "sellOrder";

            var body = new Dictionary<string, object>
            {
                { "id", "1"},
                { "method", method},
                { "params", $"{context.Rate.ToDecimalValue()},{context.Quantity.ToDecimalValue()},{context.Pair.ToTicker(this).ToLower()}"}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.id);
        }

        public Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            throw new NotImplementedException();
        }

        //TODO - SC - Not working (500 internal server error)
        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "id", "1"},
                { "method", "getOrders"},
                { "params", context.RemoteGroupId}
            };

            var rRaw = await api.QueryOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            if (r == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = r.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, r.id, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate = r.price,
                    AmountInitial = r.original_amount,
                    AmountRemaining = r.amount,
                    Market = r.symbol
                }
            };
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
                { "id", "1"},
                {"method", "createWithdraw"},
                {"params",$"{ context.Amount.ToDecimalValue()},{context.Amount.Asset.ShortCode},{context.Address.Address}" }
            };

            var rRaw = await api.PlaceWithdrawalAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.id
            };
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
