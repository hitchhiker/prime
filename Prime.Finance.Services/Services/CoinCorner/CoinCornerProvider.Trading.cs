using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.CoinCorner
{
    public partial class CoinCornerProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (rawResponse.TryGetContent(out CoinCornerSchema.ErrorResponse baseResponse))
            {
                if (!string.IsNullOrWhiteSpace(baseResponse.message))
                    throw new ApiResponseException(
                        $"Error Code: {baseResponse.message}", this, method);
            }

            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                var reason = rawResponse.ResponseMessage.ReasonPhrase;

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

            var side = context.IsBuy ? "Buy" : "Sell";

            var body = new Dictionary<string, object>
            {
                { "Amount", context.Quantity.ToDecimalValue() },
                { "Price",  context.Rate.ToDecimalValue() },
                { "CoinType", context.Rate.Asset.ShortCode}, //TODO - SC: Confirm what is difference between CoinType and Currency. No example available in API doc, nor any explanation.
                { "Currency", context.Rate.Asset.ShortCode}
            };

            var rRaw = await api.NewOrderAsync(side, body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse("");
        }

        public Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync().ConfigureAwait(false);

            CheckResponseErrors(rActiveOrdersRaw);

            var rActiveOrders = rActiveOrdersRaw.GetContent();

            var activeOrder = rActiveOrders.FirstOrDefault(x => x.OrderId.Equals(context.RemoteGroupId));

            if (activeOrder == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = activeOrder.TradeType.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, activeOrder.OrderId, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate = activeOrder.Price,
                    AmountInitial = activeOrder.Amount,
                    Market = new AssetPair(activeOrder.CoinTypeOne,activeOrder.CoinTypeTwo)
                }
            };
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
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "Amount", context.Amount.ToDecimalValue()},
                { "CoinType", context.Amount.Asset.ShortCode.ToUpper()},
                { "Address", context.Address}
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
