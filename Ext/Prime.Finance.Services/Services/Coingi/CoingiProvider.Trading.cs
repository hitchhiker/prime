using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.Coingi
{
    public partial class CoingiProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            var reason = r.ResponseMessage.ReasonPhrase;

            if (r.TryGetContent(out CoingiSchema.ErrorResponse rError))
            {
                if (rError.errors?.Length > 0)
                    throw new ApiResponseException($"Code: {rError.errors[0].code}. Error: {rError.errors[0].message}",
                        this, method);
            }
            else if (!r.ResponseMessage.IsSuccessStatusCode)
            {
                throw new ApiResponseException($"HTTP error {r.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "currencyPair", context.Pair.ToTicker(this).ToLower() },
                { "type", context.IsBuy ? "0" : "1"},
                { "price", context.Rate.ToDecimalValue() },
                { "volume", context.Quantity.ToDecimalValue() }
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.result);
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
                {"pageNumber",1 },
                {"pageSize",100 },
                { "currencyPair", context.Market.ToTicker(this).ToLower() }
            };

            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rActiveOrdersRaw);

            var activeOrders = rActiveOrdersRaw.GetContent().orders;

            var order = activeOrders.FirstOrDefault(x => x.id.Equals(context.RemoteGroupId));

            if (order == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = order.type == 0;

            return new TradeOrderStatusResponse(Network, context.RemoteGroupId, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Market = new AssetPair(order.currencyPair.baseAsset,order.currencyPair.counter),
                    Rate = order.price,
                    AmountInitial = order.originalCounterAmount,
                    AmountRemaining = order.counterAmount
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                {"currency",context.Amount.Asset.ShortCode },
                {"amount",context.Amount.ToDecimalValue() },
                { "address", context.Address.Address }
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.result == false ? null : new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
