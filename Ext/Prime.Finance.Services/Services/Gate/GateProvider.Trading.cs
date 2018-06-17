using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.Gate
{
    public partial class GateProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.TryGetContent(out GateSchema.ErrorResponse rError))
            {
                if (!rError.result)
                    throw new ApiResponseException($"Error {rError.code[0]} - {rError.message.TrimEnd('.')}", this, method);
            }

            if (!r.ResponseMessage.IsSuccessStatusCode)
                throw new ApiResponseException($"{r.ResponseMessage.ReasonPhrase} ({r.ResponseMessage.StatusCode})",
                    this, method);
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiPrivateProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "amount", context.Quantity.ToDecimalValue() },
                { "rate", context.Rate.ToDecimalValue()},
                { "currencyPair", context.Pair.ToTicker(this)}
            };

            var rRaw = await api.NewOrderAsync(context.IsBuy ? "buy" : "sell", body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(null);
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
            var api = ApiPrivateProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "orderNumber", context.RemoteGroupId },
                { "currencyPair", context.Market.ToTicker(this)}
            };

            var rRaw = await api.QueryOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            if (r?.order == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = r.order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;
            var isOpen = r.order.status.IndexOf("open", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, r.order.id, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    Rate = r.order.rate,
                    AmountInitial = r.order.initial_amount,
                    AmountRemaining = r.order.amount
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiPrivateProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                {"currency", context.Amount.Asset.ShortCode},
                {"amount", context.Amount.ToDecimalValue()},
                {"address", context.Address.Address}
            };

            var rRaw = await api.PlaceWithdrawalAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.FromNowhere);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
