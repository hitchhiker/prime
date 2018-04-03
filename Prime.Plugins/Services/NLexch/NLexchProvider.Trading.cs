using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using Prime.Plugins.Services.Kuna;
using RestEase;

namespace Prime.Plugins.Services.NLexch
{
    public partial class NLexchProvider : IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.TryGetContent(out KunaSchema.ErrorResponse rError))
                if (rError.error != null)
                    throw new ApiResponseException($"{rError.error.message.TrimEnd('.')} ({rError.error.code})", this, method);
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            string side = context.IsBuy ? "buy" : "sell";

            var body = new Dictionary<string, object>
            {
                { "market",  context.Pair.ToTicker(this).ToLower()},
                { "side",side},
                { "volume", context.Quantity.ToDecimalValue()},
                { "price", context.Rate.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.id);
        }

        public Task<TradeOrdersResponse> GetOrderHistory(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            if (!context.HasMarket)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var rRaw = await api.QueryOrderAsync(context.RemoteGroupId).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var order = rRaw.GetContent();

            if (order == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = order.side.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;
            var isOpen = order.state.IndexOf("wait", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, order.id, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    Rate = order.price,
                    Market = order.market,
                    AmountRemaining = order.remaining_volume,
                    AmountInitial = order.remaining_volume - order.executed_volume
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(true, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;
    }
}
