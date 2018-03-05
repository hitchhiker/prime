using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Plugins.Services.Poloniex
{
    // https://poloniex.com/support/api/
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class PoloniexProvider : IOrderLimitProvider
    {
        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var buy = context.IsBuy;
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var body = CreatePoloniexBody(buy ? PoloniexBodyType.LimitOrderBuy : PoloniexBodyType.LimitOrderSell);
            body.Add("currencyPair", pairCode);
            body.Add("rate", context.Rate);
            body.Add("amount", context.Quantity);

            //fillOrKill //immediateOrCancel //postOnly

            var rRaw = await api.PlaceOrderLimitAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.orderNumber, r.resultingTrades.Select(x=>x.tradeID));
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreatePoloniexBody(PoloniexBodyType.OrderStatus);

            if(!long.TryParse(context.RemoteGroupId, out var orderNumber))
                throw new ApiBaseException("Order id has to be of a long type", this);

            body.Add("orderNumber", context.RemoteGroupId);

            var rRaw = await api.GetOrderStatusAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var order = r.FirstOrDefault(x => x.globalTradeID == orderNumber);
            if(order == null)
                throw new NoTradeOrderException(context, this);

            var openOrders = await GetOpenOrders(context).ConfigureAwait(false);
            var isOpen = openOrders.Any(x => x.RemoteOrderId.Equals(context.RemoteGroupId));

            var isBuy = order.type.Equals("buy", StringComparison.OrdinalIgnoreCase);

            return new TradeOrderStatus(context.RemoteGroupId, isBuy, isOpen, false)
            {
                Market = order.currencyPair.ToAssetPair(this),
                Rate = order.rate,
                AmountInitial = order.amount,
                AmountRemaining = order.amount - order.total
            };
        }

        public async Task<IEnumerable<TradeOrderStatus>> GetOpenOrders(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreatePoloniexBody(PoloniexBodyType.OpenOrders);
            body.Add("currencyPair", "all");

            var rRaw = await api.GetOpenOrdersAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var openOrders = new List<TradeOrderStatus>();

            foreach (var rMarketOrders in r)
            {
                var market = rMarketOrders.Key.ToAssetPair(this);

                foreach (var rOrder in rMarketOrders.Value)
                {
                    var isBuy = rOrder.type.Equals("buy", StringComparison.OrdinalIgnoreCase);
                    openOrders.Add(new TradeOrderStatus(rOrder.orderNumber.ToString(), isBuy, true, false)
                    {
                        Market = market,
                        AmountInitial = rOrder.amount,
                        AmountRemaining = rOrder.amount - rOrder.total,
                        Rate = rOrder.rate
                    });
                }
            }

            return openOrders;
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;
    }
}
