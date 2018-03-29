using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.Bitsane
{
    public partial class BitsaneProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.TryGetContent(out BitsaneSchema.ErrorResponse rError))
            {
                if (rError.statusText.Equals("success", StringComparison.OrdinalIgnoreCase) == false)
                    throw new ApiResponseException($"Status Code: {rError.statusCode}. Status Text: {rError.statusText.TrimEnd('.')}. Message: {rError.result.message.TrimEnd('.')}", this, method);
            }

            if (r.TryGetContent(out BitsaneSchema.BaseResponse rBase))
            {
                if (rBase.statusText.Equals("success", StringComparison.OrdinalIgnoreCase) == false)
                    throw new ApiResponseException($"Status Code: {rBase.statusCode}. Status Text: {rBase.statusText.TrimEnd('.')}", this, method);
            }
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>()
            {
                {"currency", context.Amount.Asset.ShortCode},
                { "amount", context.Amount.ToDecimalValue().ToString()},
                { "address", context.Address.Address}
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.result.withdrawal_id
            };
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            string side = context.IsBuy ? "buy" : "sell";

            var body = new Dictionary<string, object>
            {
                { "pair",  context.Pair.ToTicker(this)},
                { "side",side},
                { "amount", context.Quantity.ToDecimalValue().ToString()},
                { "price", context.Rate.ToDecimalValue().ToString()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.result.order_id);
        }

        public Task<TradeOrdersResponse> GetTradeOrdersAsync(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            if (!context.HasMarket)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "order_id",  context.RemoteGroupId}
            };

            var rRaw = await api.QueryOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var orderResponse = rRaw.GetContent();

            if (orderResponse == null || orderResponse.result.Length == 0)
                throw new NoTradeOrderException(context, this);

            var order = orderResponse.result[0];

            var isBuy = order.side.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;
            
            return new TradeOrderStatusResponse(Network, order.id, isBuy, !order.is_closed, false)
            {
                TradeOrderStatus =
                {
                    Rate = order.price,
                    Market = order.pair,
                    AmountInitial = order.original_amount,
                    AmountRemaining = order.remaining_amount
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
