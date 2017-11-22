using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Bittrex
{
    public partial class BittrexProvider
    {
        private TradeOrderType GetTradeOrderType(string tradeOrderTypeSchema)
        {
            if (tradeOrderTypeSchema.Equals("LIMIT_BUY", StringComparison.OrdinalIgnoreCase))
                return TradeOrderType.LimitBuy;
            if (tradeOrderTypeSchema.Equals("LIMIT_SELL", StringComparison.OrdinalIgnoreCase))
                return TradeOrderType.LimitSell;
            return TradeOrderType.None;
        }

        public async Task<TradeOrders> GetOpenOrdersAsync(PrivatePairContext context)
        {
            var api = ApiProvider.GetApi(context);
            var remotePair = context.Pair.ToTicker(this);

            var r = await api.GetMarketOpenOrders().ConfigureAwait(false);

            CheckResponseErrors(r);

            var orders = new TradeOrders(Network);

            foreach (var order in r.result)
            {
                orders.Add(new TradeOrder(order.OrderUuid, Network, order.Exchange.ToAssetPair(this, '_'), GetTradeOrderType(order.Type))
                {
                    Quantity = order.Quantity,
                    QuantityRemaining = order.QuantityRemaining
                });
            }

            return orders;
        }

        public async Task<TradeOrders> GetOrderHistoryAsync(PrivatePairContext context)
        {
            var api = ApiProvider.GetApi(context);
            var remotePair = context.Pair.ToTicker(this);

            var r = await api.GetAccountHistory().ConfigureAwait(false);

            CheckResponseErrors(r);

            var orders = new TradeOrders(Network);

            foreach (var order in r.result)
            {
                orders.Add(new TradeOrder(order.OrderUuid, Network, order.Exchange.ToAssetPair(this, '_'), GetTradeOrderType(order.Type))
                {
                    Quantity = order.Quantity,
                    QuantityRemaining = order.QuantityRemaining
                });
            }

            return orders;
        }
    }
}
