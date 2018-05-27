using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.OkCoin
{
    public partial class OkCoinProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            var reason = rawResponse.ResponseMessage.ReasonPhrase;

            if (rawResponse.TryGetContent(out OkCoinSchema.ErrorResponse errorResponse))
            {
                if (!errorResponse.result)
                    throw new ApiResponseException($"{errorResponse.error_code.TrimEnd('.')}", this, method);
            }
            else
            {
                throw new ApiResponseException(
                    $"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}",
                    this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var type = context.IsBuy ? "buy" : "sell";

            var body = new Dictionary<string, object>
            {
                { "symbol", context.Pair.ToTicker(this)},
                { "type", type},
                { "price", context.Rate.ToDecimalValue()},
                { "amount", context.Quantity.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

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

            var body = new Dictionary<string, object>
            {
                { "symbol", context.Market.ToTicker(this)},
                { "order_id", context.RemoteGroupId}
            };

            var orderRaw = await api.QueryOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(orderRaw);

            var rOrder = orderRaw.GetContent();

            if (rOrder.orders == null || rOrder.orders.Length == 0 || rOrder.result == false)
            {
                throw new NoTradeOrderException(context, this);
            }

            var order = rOrder.orders[0];

            var isBuy = order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;
            var isOpen = order.status == 0;

            return new TradeOrderStatusResponse(Network, context.RemoteGroupId, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    AmountRemaining = order.amount - order.deal_amount,
                    Rate = order.price,
                    Market = context.Market
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            throw new NotImplementedException();
            //var api = ApiProviderPrivate.GetApi(context);

            //var body = CreateBody();
            //body.Add("currency", context.Amount.Asset.ShortCode);
            //body.Add("amount", context.Amount.ToDecimalValue());
            //body.Add("address", context.Address.Address);

            //var rRaw = await api.PrepareWithdrawRequestAsync(body).ConfigureAwait(false);

            //CheckResponseErrors(rRaw);

            //var r = rRaw.GetContent();

            //var transactionId = r.returnObj.transactionId;

            //var bodySubmit = CreateBody();
            //bodySubmit.Add("transactionId", transactionId);
            //var rSubmitRaw = await api.SubmitWithdrawRequestAsync(bodySubmit).ConfigureAwait(false);

            //CheckResponseErrors(rSubmitRaw);

            //var rSubmit = rSubmitRaw.GetContent();

            //return new WithdrawalPlacementResult()
            //{
            //    WithdrawalRemoteId = rSubmit.returnObj.id
            //};
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(true, CanGetOrderMarket.ByAdditionalRequest);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
