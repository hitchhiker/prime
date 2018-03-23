using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.Quoine
{
    public partial class QuoineProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                var reason = rawResponse.ResponseMessage.ReasonPhrase;

                if (rawResponse.TryGetContent(out QuoineSchema.ErrorResponse baseResponse))
                {
                    if (baseResponse.errors.Count > 0)
                    {
                        var errors = baseResponse.errors.Values.ElementAt(0);

                        if (errors.Length > 0)
                        {
                            throw new ApiResponseException($"Error: {errors[0].TrimEnd('.')}", this, method);
                        }
                    }
                }

                throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(String.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
            }
        }

        public Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);
            
            var api = ApiProvider.GetApi(context);

            PairCodeToProductId.TryGetValue(context.Pair, out var productId);

            string side = context.IsBuy ? "buy" : "sell";

            var body = new Dictionary<string, object>
            {
                {"order_type", "limit"},
                {"product_id", productId},
                {"side", side},
                {"quantity", context.Quantity.ToDecimalValue()},
                {"price", context.Rate.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.id);
        }

        public Task<TradeOrdersResponse> GetTradeOrdersAsync(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(true, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
