using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Finance.Services.Services.Vaultoro
{
    public partial class VaultoroProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                var reason = rawResponse.ResponseMessage.ReasonPhrase;

                if (rawResponse.TryGetContent(out VaultoroSchema.ErrorResponse baseResponse))
                {
                    throw new ApiResponseException($"{baseResponse.data?.message.TrimEnd('.')}", this, method);
                }

                throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProviderPrivate.GetApi(context);

            var rRaw = await api.NewOrderAsync(context.IsBuy ? "buy" : "sell", context.Rate.Asset.ShortCode.ToLower(), context.Quantity.ToDecimalValue(), context.Rate.ToDecimalValue()).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.Order_ID);
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var rOrdersRaw = await api.QueryOrdersAsync().ConfigureAwait(false);
            CheckResponseErrors(rOrdersRaw);

            var rOrders = rOrdersRaw.GetContent();

            var buyOrdersList = rOrders.data.FirstOrDefault(x => x.b != null && x.b.Length > 0);
            var sellOrdersList = rOrders.data.FirstOrDefault(x => x.s != null && x.s.Length > 0);

            if (buyOrdersList == null && sellOrdersList == null)
                throw new NoTradeOrderException(context, this);

            var buyOrder = buyOrdersList?.b.FirstOrDefault(x => x.Order_ID.Equals(context.RemoteGroupId));
            var sellOrder = sellOrdersList?.s.FirstOrDefault(x => x.Order_ID.Equals(context.RemoteGroupId));

            if (buyOrder == null && sellOrder == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = buyOrder != null;

            return new TradeOrderStatusResponse(Network, isBuy ? buyOrder.Order_ID : sellOrder.Order_ID, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate =  isBuy ? buyOrder.Gold_Price : sellOrder.Gold_Price,
                    AmountRemaining = isBuy ? buyOrder.Gold_Amount : sellOrder.Gold_Amount
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

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var rRaw = await api.SubmitWithdrawRequestAsync(context.Amount.ToDecimalValue()).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            //No id returned.
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.FromNowhere);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
