﻿using System;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;

namespace Prime.Plugins.Services.Common
{
    public abstract partial class CommonProviderTiLiWe<TApi> : IOrderLimitProvider, IBalanceProvider
    {
        public virtual async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreatePostBody();
            body.Add("method", ApiMethodsConfig[ApiMethodNamesTiLiWe.GetInfoExt]);

            var r = await api.GetUserInfoExtAsync(body).ConfigureAwait(false);

            CheckResponse(r);

            var balances = new BalanceResults(this);

            foreach (var fund in r.return_.funds)
            {
                var c = fund.Key.ToAsset(this);
                balances.Add(c, fund.Value.value, fund.Value.inOrders);
            }

            return balances;
        }

        public virtual async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreatePostBody();
            body.Add("method", ApiMethodsConfig[ApiMethodNamesTiLiWe.Trade]);
            body.Add("pair", context.Pair.ToTicker(this).ToLower());
            body.Add("type", context.IsBuy ? "buy" : "sell");
            body.Add("rate", context.Rate.ToDecimalValue());
            body.Add("amount", context.Quantity.ToDecimalValue());

            var r = await api.TradeAsync(body).ConfigureAwait(false);

            CheckResponse(r);

            return new PlacedOrderLimitResponse(r.return_.order_id.ToString());
        }

        public Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreatePostBody();
            body.Add("method", ApiMethodsConfig[ApiMethodNamesTiLiWe.OrderInfo]);
            body.Add("order_id", context.RemoteGroupId);

            var r = await api.GetOrderInfoAsync(body).ConfigureAwait(false);

            CheckResponse(r);

            if (r.return_.Count == 0 || !r.return_.TryGetValue(context.RemoteGroupId, out var order))
                throw new NoTradeOrderException(context, this);

            var isBuy = order.type.Equals("buy", StringComparison.OrdinalIgnoreCase);

            return new TradeOrderStatusResponse(Network, context.RemoteGroupId, isBuy, order.status == 0, order.status == 2 || order.status == 3)
            {
                TradeOrderStatus =
                {
                    Market = order.pair.ToAssetPair(this),
                    Rate = order.rate,
                    AmountInitial = order.start_amount,
                    AmountRemaining = order.amount
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        public virtual MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;
    }
}
