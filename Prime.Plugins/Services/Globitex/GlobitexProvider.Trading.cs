using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using Prime.Plugins.Services.Common;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.Globitex
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://globitex.com/api/
    public partial class GlobitexProvider : IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                if (rawResponse.TryGetContent(out GlobitexSchema.ErrorResponse baseResponse))
                {
                    if (baseResponse.errors.Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(baseResponse.errors[0].data))
                            throw new ApiResponseException($"Error Code: {baseResponse.errors[0].code} - {baseResponse.errors[0].message.TrimEnd('.')} - {baseResponse.errors[0].data.TrimEnd('.')}", this, method);

                        throw new ApiResponseException($"Error Code: {baseResponse.errors[0].code} - {baseResponse.errors[0].message.TrimEnd('.')}", this, method);
                    }
                }

                var reason = rawResponse.ResponseMessage.ReasonPhrase;
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
                { "price", context.Rate.ToDecimalValue().ToString(CultureInfo.InvariantCulture)},
                { "quantity",  context.Quantity.ToDecimalValue().ToString(CultureInfo.InvariantCulture)  }
            };

            var rRaw = await api.PlaceNewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.ExecutionReport.orderId);
        }

        public async Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rOrdersRaw = await api.GetMyTradesAsync().ConfigureAwait(false);
            CheckResponseErrors(rOrdersRaw);

            var rOrders = rOrdersRaw.GetContent();

            var orders = new List<TradeOrderStatus>();

            foreach (var order in rOrders.trades)
            {
                var isBuy = order.side.Equals("buy", StringComparison.OrdinalIgnoreCase);
                orders.Add(new TradeOrderStatus(Network, order.clientOrderId, isBuy, false, false)
                {
                    Market = order.symbol.ToAssetPair(this),
                    AmountInitial = order.execQuantity,
                    Rate = order.execPrice
                });
            }
            
            return new TradeOrdersResponse(orders);
        }

        public async Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rOrdersRaw = await api.GetActiveOrdersAsync().ConfigureAwait(false);
            CheckResponseErrors(rOrdersRaw);

            var rOrders = rOrdersRaw.GetContent();

            var orders = new List<TradeOrderStatus>();

            foreach (var order in rOrders.orders)
            {
                var isBuy = order.side.Equals("buy", StringComparison.OrdinalIgnoreCase);
                orders.Add(new TradeOrderStatus(Network, order.clientOrderId, isBuy, true, false)
                {
                    Market = order.symbol.ToAssetPair(this),
                    AmountInitial = order.orderQuantity,
                    Rate = order.orderPrice
                });
            }
            
            return new OpenOrdersResponse(orders);
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

            return new TradeOrderStatusResponse(Network, activeOrder.orderId, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate = activeOrder.orderPrice,
                    AmountInitial = activeOrder.execQuantity,
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
