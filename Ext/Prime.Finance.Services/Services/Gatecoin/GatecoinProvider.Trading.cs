using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.Gatecoin
{
    public partial class GatecoinProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            var reason = rawResponse.ResponseMessage.ReasonPhrase;

            if (rawResponse.TryGetContent(out GatecoinSchema.BaseResponse baseResponse))
            {
                if (!baseResponse.responseStatus.message.Equals("OK", StringComparison.OrdinalIgnoreCase))
                    throw new ApiResponseException($"Error Code: {baseResponse.responseStatus.message.TrimEnd('.')}", this, method);
            }

            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var side = context.IsBuy ? "Bid" : "Ask";

            var body = new Dictionary<string, object>
            {
                { "Code", context.Pair.ToTicker(this)},
                { "Way", side},
                { "Price", context.Rate.ToDecimalValue()},
                { "Amount", context.Quantity.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.clOrderId);
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
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.QueryOrderAsync(context.RemoteGroupId).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            if (r == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = r.side == 0;

            return new TradeOrderStatusResponse(Network, r.clOrderId, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate = r.price,
                    AmountInitial = r.initialQuantity,
                    AmountRemaining = r.remainingQuantity,
                    Market = r.code
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
                {"Amount", context.Amount.ToDecimalValue()},
                {"DigiCurrency", context.Amount.Asset.ShortCode},
                {"AddressName", context.Address.Address},
                {"Comment","Comment" }
            };

            var rRaw = await api.PlaceWithdrawalAsync(body, context.Amount.ToDecimalValue().ToString()).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
