using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Finance.Services.Services.Ccex
{
    public partial class CcexProvider : IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            var reason = rawResponse.ResponseMessage.ReasonPhrase;

            if (rawResponse.TryGetContent(out CcexSchema.ErrorResponse errorResponse))
            {
                if (!errorResponse.success)
                    throw new ApiResponseException($"{errorResponse.message.TrimEnd('.')}", this, method);
            }
            else
            {
                throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);
            
            var rRaw = context.IsBuy
                ? await api.PlaceBuyOrderAsync(context.Pair.ToTicker(this).ToLower(), context.Quantity.ToDecimalValue(), context.Rate).ConfigureAwait(false)
                : await api.PlaceSellOrderAsync(context.Pair.ToTicker(this).ToLower(), context.Quantity.ToDecimalValue(), context.Rate).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.result.uuid);
        }
        
        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var ordersRaw = await api.QueryOrderAsync(context.RemoteGroupId).ConfigureAwait(false);

            CheckResponseErrors(ordersRaw);

            var rOrders = ordersRaw.GetContent();

            if (rOrders.Length == 0)
            {
                throw new NoTradeOrderException(context, this);
            }

            var order = rOrders[0];

            var isBuy = order.Type.IndexOf("LIMIT_BUY", StringComparison.OrdinalIgnoreCase) >= 0;
            var isOpen = order.IsOpen.IndexOf("true", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, order.OrderUuid, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    AmountInitial = order.Quantity,
                    AmountRemaining = order.QuantityRemaining,
                    Rate = order.Price,
                    Market = order.Exchange.ToAssetPair(this)
                }
            };
        }

        public Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            throw new NotImplementedException();
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
