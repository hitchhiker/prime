using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.Exmo
{
    public partial class ExmoProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        //private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        //{
        //    if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
        //    {
        //        var reason = rawResponse.ResponseMessage.ReasonPhrase;

        //        if (rawResponse.TryGetContent(out ExmoSchema.ErrorResponse baseResponse))
        //        {
        //            throw new ApiResponseException($"Error Code: {baseResponse.error.code} - {baseResponse.error.message.TrimEnd('.')}", this, method);

        //        }

        //        throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
        //    }
        //}

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var side = context.IsBuy ? "buy" : "sell";

            var body = new Dictionary<string, object>
            {
                { "pair", context.Pair.ToTicker(this).ToLower()},
                { "type", side},
                { "price", context.Rate.ToDecimalValue()},
                { "quantity", context.Quantity.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            //CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.order_id);
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
            
            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync().ConfigureAwait(false);

            //CheckResponseErrors(rActiveOrdersRaw);

            var rActiveOrders = rActiveOrdersRaw.GetContent();

            ExmoSchema.OrderInfoResponse order = null;

            foreach (var orderPair in rActiveOrders)
            {
                order = orderPair.Value.FirstOrDefault(x => x.order_id.Equals(context.RemoteGroupId));

                if (order != null)
                {
                    break;
                }
            }
            
            if (order == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, order.order_id, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate = order.price,
                    AmountInitial = order.amount,
                    Market = order.pair
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
                {"amount", context.Amount.ToDecimalValue()},
                {"currency", context.Amount.Asset.ShortCode},
                {"address", context.Address.Address}
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            //CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.task_id
            };
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
