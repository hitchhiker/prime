using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Finance.Services.Services.Yobit
{
    public partial class YobitProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private Dictionary<string, object> CreateBody()
        {
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp());

            var body = new Dictionary<string, object>()
            {
                { "nonce", timeStamp }
            };

            return body;
        }

        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.TryGetContent(out YobitSchema.ErrorResponse rError))
                if (!rError.success)
                    throw new ApiResponseException(rError.error, this, method);

            if (!r.ResponseMessage.IsSuccessStatusCode)
                throw new ApiResponseException($"{r.ResponseMessage.ReasonPhrase} ({r.ResponseMessage.StatusCode})",
                    this, method);

            if (r.GetContent() is YobitSchema.BaseResponse<T> baseResponse)
                if (!baseResponse.success)
                    throw new ApiResponseException("API response error occurred", this, method);
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreateBody();
            body.Add("method", "Trade");
            body.Add("pair", context.Pair.ToTicker(this));
            body.Add("type", context.IsBuy ? "buy" : "sell");
            body.Add("amount", context.Quantity);
            body.Add("rate", context.Rate.ToDecimalValue());

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.returnData.order_id);
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
            var api = ApiProviderPrivate.GetApi(context);

            var order = await GetOrderReponseByIdAsync(context).ConfigureAwait(false);
            
            var bodyActiveOrders = CreateBody();
            bodyActiveOrders.Add("method", "ActiveOrders");
            bodyActiveOrders.Add("pair", order.pair);

            // Checks if this order is contained in active list.
            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync(bodyActiveOrders).ConfigureAwait(false);
            CheckResponseErrors(rActiveOrdersRaw);

            var activeOrders = rActiveOrdersRaw.GetContent().returnData;
            // If the active list contains this order and the request for active orders was successful, then it is active. Otherwise it is not active.
            var isOpen = activeOrders.ContainsKey(context.RemoteGroupId);

            var isBuy = order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, context.RemoteGroupId, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    Market = order.pair.ToAssetPair(this),
                    Rate = order.rate,
                    AmountInitial = order.start_amount
                }
            };
        }

        private async Task<YobitSchema.OrderInfoEntryResponse> GetOrderReponseByIdAsync(RemoteIdContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);
            
            var bodyOrderInfo = CreateBody();
            bodyOrderInfo.Add("method", "OrderInfo");
            bodyOrderInfo.Add("order_id", context.RemoteGroupId);

            var rOrderRaw = await api.QueryOrderInfoAsync(bodyOrderInfo).ConfigureAwait(false);
            CheckResponseErrors(rOrderRaw);

            var orderContent = rOrderRaw.GetContent();
            if (!orderContent.returnData.Key.Equals(context.RemoteGroupId))
                throw new NoTradeOrderException(context, this);

            var order = orderContent.returnData.Value;

            return order;
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);
            
            var body = CreateBody();
            body.Add("method", "WithdrawCoinsToAddress");
            body.Add("coinName", context.Amount.Asset.ShortCode);
            body.Add("amount", context.Amount.ToDecimalValue());
            body.Add("address", context.Address.Address);

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            // No id is returned from exchange.
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
