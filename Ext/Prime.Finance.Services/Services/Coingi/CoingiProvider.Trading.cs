using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
            //var api = ApiProvider.GetApi(context);

            //var order = await GetOrderReponseByIdAsync(context).ConfigureAwait(false);

            //var bodyActiveOrders = CreateBody();
            //bodyActiveOrders.Add("method", "ActiveOrders");
            //bodyActiveOrders.Add("pair", order.pair);

            //// Checks if this order is contained in active list.
            //var rActiveOrdersRaw = await api.QueryActiveOrdersAsync(bodyActiveOrders).ConfigureAwait(false);
            //CheckResponseErrors(rActiveOrdersRaw);

            //var activeOrders = rActiveOrdersRaw.GetContent().returnData;
            //// If the active list contains this order and the request for active orders was successful, then it is active. Otherwise it is not active.
            //var isOpen = activeOrders.ContainsKey(context.RemoteGroupId);

            //var isBuy = order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            //return new TradeOrderStatusResponse(Network, context.RemoteGroupId, isBuy, isOpen, false)
            //{
            //    TradeOrderStatus =
            //    {
            //        Market = order.pair.ToAssetPair(this),
            //        Rate = order.rate,
            //        AmountInitial = order.start_amount
            //    }
            //};
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            throw new NotImplementedException();

            //var api = ApiProvider.GetApi(context);

            //var body = CreateBody();
            //body.Add("method", "WithdrawCoinsToAddress");
            //body.Add("coinName", context.Amount.Asset.ShortCode);
            //body.Add("amount", context.Amount.ToDecimalValue());
            //body.Add("address", context.Address.Address);

            //var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            //CheckResponseErrors(rRaw);

            //// No id is returned from exchange.
            //return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
