using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.Coinfloor
{
    public partial class CoinfloorProvider : IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                var reason = rawResponse.ResponseMessage.ReasonPhrase;

                if (rawResponse.TryGetContent(out CoinfloorSchema.ErrorResponse baseResponse))
                {
                    throw new ApiResponseException($"Error Code: {baseResponse.error_code} - {baseResponse.error_msg.TrimEnd('.')}", this, method);
                }

                throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var side = context.IsBuy ? "buy" : "sell";

            var body = new Dictionary<string, object>
            {
                { "amount", context.Quantity.ToDecimalValue()},
                { "price", context.Rate.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(context.Pair.ToTicker(this, '/'), side, body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.id);
        }

        public Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync(context.Market.ToTicker(this, '/')).ConfigureAwait(false);

            CheckResponseErrors(rActiveOrdersRaw);

            var rActiveOrders = rActiveOrdersRaw.GetContent();

            var activeOrder = rActiveOrders.FirstOrDefault(x => x.id.Equals(context.RemoteGroupId));

            if (activeOrder == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = activeOrder.type == 0;

            return new TradeOrderStatusResponse(Network, activeOrder.id, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate = activeOrder.price,
                    AmountInitial = activeOrder.amount,
                    Market = context.Market
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
        
        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(true, CanGetOrderMarket.FromNowhere);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
