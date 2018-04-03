using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Bittrex
{
    // https://bittrex.com/home/api
    public partial class BittrexProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        public MinimumTradeVolume[] MinimumTradeVolume { get; } = { new MinimumTradeVolume() { MinimumSell = 0.011m, MinimumBuy = 0.011m } }; //50K Satoshi /4 USD

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        private TradeOrderType GetTradeOrderType(string tradeOrderTypeSchema)
        {
            if (tradeOrderTypeSchema.Equals("LIMIT_BUY", StringComparison.OrdinalIgnoreCase))
                return TradeOrderType.LimitBuy;
            if (tradeOrderTypeSchema.Equals("LIMIT_SELL", StringComparison.OrdinalIgnoreCase))
                return TradeOrderType.LimitSell;
            return TradeOrderType.None;
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);
            var remotePair = context.Pair.ToTicker(this);

            var quantity = context.Quantity.ToDecimalValue();
            var rate = context.Rate.ToDecimalValue();

            var rRaw = context.IsSell ?
                await api.PlaceMarketSellLimit(remotePair, quantity, rate).ConfigureAwait(false) :
                await api.PlaceMarketBuyLimit(remotePair, quantity, rate).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.result.uuid);
        }

        public async Task<TradeOrdersResponse> GetOrderHistory(TradeOrdersContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rHistoryOrdersRaw = await api.GetOrderHistory().ConfigureAwait(false);
            var rOpenOrdersRaw = await api.GetMarketOpenOrders().ConfigureAwait(false);

            CheckResponseErrors(rHistoryOrdersRaw);
            CheckResponseErrors(rOpenOrdersRaw);

            var rHistoryOrders = rHistoryOrdersRaw.GetContent();
            var rOpenOrders = rOpenOrdersRaw.GetContent();

            var orders = new List<TradeOrderStatus>();
            orders.AddRange(GetTradeOrderStatusFromResponse(rHistoryOrders.result.Select(x => x as BittrexSchema.OrderCommonBase).ToList()));
            orders.AddRange(GetTradeOrderStatusFromResponse(rOpenOrders.result.Select(x => x as BittrexSchema.OrderCommonBase).ToList(), true, order => ((BittrexSchema.OpenOrderEntry) order).CancelInitiated));

            return new TradeOrdersResponse(orders);
        }

        public async Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            var api = ApiProvider.GetApi(context);
            
            var rOpenOrdersRaw = await api.GetMarketOpenOrders().ConfigureAwait(false);
            CheckResponseErrors(rOpenOrdersRaw);

            var rOpenOrders = rOpenOrdersRaw.GetContent();

            var orders = new List<TradeOrderStatus>();
            orders.AddRange(GetTradeOrderStatusFromResponse(rOpenOrders.result.Select(x => x as BittrexSchema.OrderCommonBase).ToList(), true, order => ((BittrexSchema.OpenOrderEntry) order).CancelInitiated));
            
            return new OpenOrdersResponse(orders);
        }

        private List<TradeOrderStatus> GetTradeOrderStatusFromResponse(List<BittrexSchema.OrderCommonBase> orders, bool isOpen = false, Func<BittrexSchema.OrderCommonBase, bool> checkCancelRequested = null)
        {
            var orderStatuses = new List<TradeOrderStatus>();

            foreach (var order in orders)
            {
                var isBuy = order.OrderType.Equals("LIMIT_BUY", StringComparison.OrdinalIgnoreCase);
                orderStatuses.Add(new TradeOrderStatus(Network, order.OrderUuid, isBuy, isOpen, checkCancelRequested?.Invoke(order) ?? false)
                {
                    Rate = order.Limit,
                    Market = order.Exchange.ToAssetPair(this),
                    AmountInitial = order.Quantity,
                    AmountRemaining = order.QuantityRemaining
                });
            }

            return orderStatuses;
        }

        // TODO: AY: HH, check if it's needed.
        [Obsolete]
        public async Task<TradeOrders> GetOpenOrdersAsync(PrivatePairContext context)
        {
            var api = ApiProvider.GetApi(context);
            var remotePair = context.RemotePairOrNull(this);
            
            var rRaw = await api.GetMarketOpenOrders(remotePair).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var orders = new TradeOrders(Network);
            foreach (var order in r.result)
            {
                orders.Add(new TradeOrder(order.OrderUuid, Network, order.Exchange.ToAssetPair(this), GetTradeOrderType(order.Type), order.Price)
                {
                    Quantity = order.Quantity,
                    QuantityRemaining = order.QuantityRemaining
                });
            }

            return orders;
        }

        // TODO: AY: HH, check if it's needed.
        [Obsolete]
        public async Task<TradeOrders> GetOrderHistoryAsync(PrivatePairContext context)
        {
            var api = ApiProvider.GetApi(context);
            var remotePair = context.RemotePairOrNull(this);

            var rRaw = await api.GetOrderHistory(remotePair).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var orders = new TradeOrders(Network);
            foreach (var order in r.result)
            {
                orders.Add(new TradeOrder(order.OrderUuid, Network, order.Exchange.ToAssetPair(this), GetTradeOrderType(order.Type), order.Price)
                {
                    Quantity = order.Quantity,
                    QuantityRemaining = order.QuantityRemaining
                });
            }

            return orders;
        }

        // TODO: AY: HH, check if it's needed.
        [Obsolete]
        public async Task<TradeOrder> GetOrderDetails(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetAccountOrder(context.RemoteGroupId).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var order = r.result;

            return new TradeOrder(order.OrderUuid, Network, order.Exchange.ToAssetPair(this), GetTradeOrderType(order.Type), order.Price)
            {
                Quantity = order.Quantity,
                QuantityRemaining = order.QuantityRemaining
            };
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetAccountOrder(context.RemoteGroupId).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var order = r.result;

            var isBuy = order.Type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, order.OrderUuid, isBuy, order.IsOpen, order.CancelInitiated)
            {
                TradeOrderStatus =
                {
                    Market = order.Exchange.ToAssetPair(this),
                    Rate = order.Limit,
                    AmountInitial = order.Quantity,
                    AmountRemaining = order.QuantityRemaining
                }
            };
        }

        public bool IsWithdrawalFeeIncluded => true;
        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = context.HasDescription
                ? await api.Withdraw(context.Amount.Asset.ToRemoteCode(this), context.Amount.ToDecimalValue(),
                    context.Address.Address, context.Description).ConfigureAwait(false)
                : await api.Withdraw(context.Amount.Asset.ToRemoteCode(this), context.Amount.ToDecimalValue(),
                    context.Address.Address).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.result.uuid
            };
        }
    }
}
