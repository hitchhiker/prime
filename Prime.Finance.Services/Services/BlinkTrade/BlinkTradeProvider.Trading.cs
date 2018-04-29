using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Finance;
using RestEase;

namespace Prime.Finance.Services.Services.BlinkTrade
{
    public partial class BlinkTradeProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (rawResponse.TryGetContent(out BlinkTradeSchema.ErrorResponse baseResponse))
            {
                throw new ApiResponseException(
                    $"Error: {baseResponse.Status} - {baseResponse.Description.TrimEnd('.')}", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiPrivateProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                {"MsgType", "D"},
                {"ClOrdID", 1},
                {"Symbol", context.Pair.ToTicker(this)},
                {"Side", context.IsBuy ? "1" : "2"},
                {"OrdType", "2"},
                {"Price", context.Rate.ToInt64(null)},
                {"OrderQty", context.Quantity.ToInt32(null)},
                {"BrokerID", 5}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.OrderID);
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
                {"MsgType", "U4"},
                {"OrdersReqID", 1}
            };

            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rActiveOrdersRaw);

            var rActiveOrders = rActiveOrdersRaw.GetContent();

            var activeOrder = rActiveOrders.OrdListGrp?.FirstOrDefault(x => x.OrderID.Equals(context.RemoteGroupId));

            if (activeOrder == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = activeOrder.Side == 1;

            return new TradeOrderStatusResponse(Network, activeOrder.OrderID, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate = activeOrder.Price,
                    Market = activeOrder.Symbol.ToAssetPair(this)
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            //TODO: SC - This needs to be implemented
            throw new NotImplementedException();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
