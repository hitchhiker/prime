using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.Acx
{
    public partial class AcxProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.TryGetContent(out AcxSchema.ErrorResponse rError))
                if (rError.error != null)
                    throw new ApiResponseException($"Code: {rError.error.code}. Message: {rError.error.message}", this, method);
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "market", context.Pair.ToTicker(this).ToLower() },
                { "side", context.IsBuy ? "buy" : "sell"},
                { "volume", context.Quantity.ToDecimalValue()},
                { "price", context.Rate.ToDecimalValue()}
            };

            //TODO: SC - Documentation does not show example of response, so this has to be tested with real money to see what the endpoint returns
            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);
            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.id);
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            string market = context.Market.ToTicker(this).ToLower();

            var order = await GetOrderReponseByIdAsync(context, market).ConfigureAwait(false);

            var isOpen = order.state.IndexOf("wait", StringComparison.OrdinalIgnoreCase) >= 0;

            var isBuy = order.side.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatus(order.id, isBuy, isOpen, false)
            {
                Rate = order.price,
                Market = market
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        private async Task<AcxSchema.OrderInfoResponse> GetOrderReponseByIdAsync(RemoteIdContext context, string market)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "id", context.RemoteGroupId}
            };

            var rRaw = await api.GetOrderInfoAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var order = rRaw.GetContent();

            if (order == null)
            {
                throw new NoTradeOrderException(context.RemoteGroupId, this);
            }

            return order;
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);
            
            var body = new Dictionary<string, object>
            {
                {"currency", context.Amount.Asset.ShortCode.ToLower()},
                {"sum", context.Amount.ToDecimalValue()},
                {"address", context.Address.Address}
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);
            
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.FromNowhere);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
