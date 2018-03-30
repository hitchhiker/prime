using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.Globitex
{
    public partial class GlobitexProvider : IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                var reason = rawResponse.ResponseMessage.ReasonPhrase;

                if (rawResponse.TryGetContent(out GlobitexSchema.ErrorResponse baseResponse))
                {
                    if (baseResponse.errors.Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(baseResponse.errors[0].data))
                            throw new ApiResponseException($"Error Code: {baseResponse.errors[0].code} - {baseResponse.errors[0].message.TrimEnd('.')} - {baseResponse.errors[0].data.TrimEnd('.')}", this, method);

                        throw new ApiResponseException($"Error Code: {baseResponse.errors[0].code} - {baseResponse.errors[0].message.TrimEnd('.')}", this, method);
                    }
                }

                throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var side = context.IsBuy ? "buy" : "sell";

            var body = new Dictionary<string, object>
            {
                { "symbol", context.Pair.ToTicker(this).ToUpper()},
                { "side", side},
                { "price", context.Rate.ToDecimalValue().ToString()},
                { "quantity",  context.Quantity.ToDecimalValue().ToString()  }
            };

            var rRaw = await api.PlaceNewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.ExecutionReport.orderId);
        }

        public Task<TradeOrdersResponse> GetTradeOrdersAsync(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);
           
            var rActiveOrdersRaw = await api.GetActiveOrdersAsync().ConfigureAwait(false);
            CheckResponseErrors(rActiveOrdersRaw);

            var rActiveOrders = rActiveOrdersRaw.GetContent();

            var activeOrder = rActiveOrders.orders.FirstOrDefault(x => x.orderId.Equals(context.RemoteGroupId));

            if (activeOrder == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = activeOrder.side.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(activeOrder.orderId, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate = activeOrder.orderPrice,
                    Market = activeOrder.symbol.ToAssetPair(this)
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => throw new NotImplementedException();

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
