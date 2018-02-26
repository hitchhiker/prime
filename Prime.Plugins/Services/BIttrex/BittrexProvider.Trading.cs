using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Bittrex
{
    public partial class BittrexProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        // TODO: AY: BittrexProvider, review MinimumTradeVolume.
        public MinimumTradeVolume[] MinimumTradeVolume { get; } = { new MinimumTradeVolume() { MinimumSell = 0.011m, MinimumBuy = 0.011m } }; //50K Satoshi /4 USD

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

            var r = context.IsSell ?
                await api.GetMarketSellLimit(remotePair, context.Quantity, context.Rate).ConfigureAwait(false) :
                await api.GetMarketBuyLimit(remotePair, context.Quantity, context.Rate).ConfigureAwait(false);

            CheckResponseErrors(r);

            return new PlacedOrderLimitResponse(r.result.uuid);
        }

        public async Task<TradeOrders> GetOpenOrdersAsync(PrivatePairContext context)
        {
            var api = ApiProvider.GetApi(context);
            var remotePair = context.RemotePairOrNull(this);
            var r = await api.GetMarketOpenOrders(remotePair).ConfigureAwait(false);

            CheckResponseErrors(r);

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

        public async Task<TradeOrders> GetOrderHistoryAsync(PrivatePairContext context)
        {
            var api = ApiProvider.GetApi(context);
            var remotePair = context.RemotePairOrNull(this);
            var r = await api.GetAccountHistory(remotePair).ConfigureAwait(false);

            CheckResponseErrors(r);

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

        public async Task<TradeOrder> GetOrderDetails(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountOrder(context.RemoteGroupId).ConfigureAwait(false);
            var order = r.result;

            CheckResponseErrors(r);

            return new TradeOrder(order.OrderUuid, Network, order.Exchange.ToAssetPair(this), GetTradeOrderType(order.Type), order.Price)
            {
                Quantity = order.Quantity,
                QuantityRemaining = order.QuantityRemaining
            };
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountOrder(context.RemoteGroupId).ConfigureAwait(false);

            CheckResponseErrors(r);

            return r?.result == null ? new TradeOrderStatus() : new TradeOrderStatus(r.result.OrderUuid, r.result.IsOpen, r.result.CancelInitiated)
            {
                AmountInitial = r.result.Quantity,
                AmountRemaining = r.result.QuantityRemaining
            };
        }

        public bool IsWithdrawalFeeIncluded => true;
        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = context.HasDescription
                ? await api.Withdraw(context.Amount.Asset.ToRemoteCode(this), context.Amount.ToDecimalValue(),
                    context.Address.Address, context.Description).ConfigureAwait(false)
                : await api.Withdraw(context.Amount.Asset.ToRemoteCode(this), context.Amount.ToDecimalValue(),
                    context.Address.Address).ConfigureAwait(false);

            CheckResponseErrors(r);

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.result.uuid
            };
        }
    }
}
