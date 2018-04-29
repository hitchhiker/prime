using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Core.Api.Request.Response;
using RestEase;

namespace Prime.Finance.Services.Services.Kuna
{
    partial class KunaProvider : IOrderLimitProvider
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
            if (!context.HasMarket)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);
            
            var rRaw = await api.QueryActiveOrdersAsync(context.Market.ToTicker(this).ToLower()).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var orders = rRaw.GetContent();

            var order = orders.FirstOrDefault(x => x.id.Equals(context.RemoteGroupId));

            if (order == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = order.side.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, order.id, isBuy, true, false)
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