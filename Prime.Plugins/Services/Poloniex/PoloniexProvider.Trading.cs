using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Plugins.Services.Poloniex
{
    // https://poloniex.com/support/api/
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class PoloniexProvider : IOrderLimitProvider
    {
        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, '_');

            var r = context.MaxRecordsCount == Int32.MaxValue
                ? await api.GetOrderBookAsync(pairCode).ConfigureAwait(false)
                : await api.GetOrderBookAsync(pairCode, context.MaxRecordsCount).ConfigureAwait(false);

            if (r.bids == null || r.asks == null)
                throw new AssetPairNotSupportedException(context.Pair, this);

            var orderBook = new OrderBook(Network, context.Pair.Reversed); //POLONIEX IS REVERSING THE MARKET

            foreach (var i in r.bids)
                orderBook.AddBid(i[0], i[1], true); //HH: Confirmed reversed on https://poloniex.com/exchange#btc_btcd

            foreach (var i in r.asks)
                orderBook.AddAsk(i[0], i[1], true);

            return orderBook.AsPair(context.Pair);
        }

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

            var r = await api.PlaceOrderLimitAsync(body).ConfigureAwait(false);

            return new PlacedOrderLimitResponse(r.orderNumber, r.resultingTrades.Select(x=>x.tradeID));
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreatePoloniexBody(PoloniexBodyType.OrderStatus);
            body.Add("orderNumber", context.RemoteGroupId);

            var r = await api.GetOrderStatusAsync(body).ConfigureAwait(false);
            
            // TODO: AY: implement, use all markets - Poloniex.

            return new TradeOrderStatus()
            {
                
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;
    }
}
