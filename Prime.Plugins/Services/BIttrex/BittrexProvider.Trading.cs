using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Bittrex
{
    // https://bittrex.com/home/api
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class BittrexProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public MinimumTradeVolume[] MinimumTradeVolume { get; } = { new MinimumTradeVolume() { MinimumSell = 0.011m, MinimumBuy = 0.011m } }; //50K Satoshi /4 USD

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

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

        public async Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetOrderHistory().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var orders =
                GetTradeOrderStatusFromResponse(r.result.Select(x => x as BittrexSchema.OrderCommonBase)
                    .ToList());

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
