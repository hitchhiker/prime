using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Core.Api.Request.Response;
using Prime.Finance.Services.Services.Bitso;
using RestEase;

namespace Prime.Finance.Services.Services.Dsx
{
    public partial class DsxProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            var reason = rawResponse.ResponseMessage.ReasonPhrase;

            if (rawResponse.TryGetContent(out DsxSchema.ErrorResponse errorResponse))
            {
                if (errorResponse.success == 0)
                    throw new ApiResponseException($"{errorResponse.error.TrimEnd('.')}", this, method);
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

            var api = ApiProviderPrivate.GetApi(context);

            var type = context.IsBuy ? "Buy" : "Sell";

            var body = CreateBody();
            body.Add("pair", context.Pair.ToTicker(this).ToLower());
            body.Add("type", type);
            body.Add("rate", context.Rate.ToDecimalValue());
            body.Add("volume", context.Quantity.ToDecimalValue());
            body.Add("orderType", "limit");

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.returnObj.orderId);
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

            var body = CreateBody();
            body.Add("orderId", context.RemoteGroupId);

            var orderRaw = await api.QueryOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(orderRaw);

            var rOrder = orderRaw.GetContent();

            if (rOrder.returnObj == null)
            {
                throw new NoTradeOrderException(context, this);
            }

            var order = rOrder.returnObj;

            var isBuy = order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;
            var isOpen = order.status == 0;

            return new TradeOrderStatusResponse(Network, context.RemoteGroupId, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    AmountRemaining = order.remainingVolume,
                    Rate = order.rate,
                    Market = order.pair.ToAssetPair(this)
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            //TODO: SC - At the time of writing, this endpoint does not appear to be functioning in the API. Possibly due to an account-related issue.
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreateBody();
            body.Add("currency", context.Amount.Asset.ShortCode);
            body.Add("amount", context.Amount.ToDecimalValue());
            body.Add("address", context.Address.Address);

            var rRaw = await api.PrepareWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var transactionId = r.returnObj.transactionId;
            
            var bodySubmit = CreateBody();
            bodySubmit.Add("transactionId", transactionId);
            var rSubmitRaw = await api.SubmitWithdrawRequestAsync(bodySubmit).ConfigureAwait(false);

            CheckResponseErrors(rSubmitRaw);

            var rSubmit = rSubmitRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = rSubmit.returnObj.id
            };
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
