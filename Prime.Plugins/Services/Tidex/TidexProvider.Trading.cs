using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;

namespace Prime.Plugins.Services.Tidex
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://tidex.com/public-api
    public partial class TidexProvider : IOrderLimitProvider, IBalanceProvider
    {
        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreateTidexPostBody();
            body.Add("method", "getInfoExt");

            var r = await api.GetUserInfoExtAsync(body).ConfigureAwait(false);

            CheckTidexResponse(r);

            var balances = new BalanceResults();

            foreach (var fund in r.return_.funds)
            {
                var c = fund.Key.ToAsset(this);
                balances.Add(c, fund.Value.value, fund.Value.inOrders);
            }

            return balances;
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreateTidexPostBody();
            body.Add("method", "Trade");
            body.Add("pair", context.Pair.ToTicker(this).ToLower());
            body.Add("type", context.IsBuy ? "buy": "sell");
            body.Add("rate", context.Rate.ToDecimalValue());
            body.Add("amount", context.Quantity);

            var r = await api.TradeAsync(body).ConfigureAwait(false);

            CheckTidexResponse(r);

            return new PlacedOrderLimitResponse(r.return_.order_id.ToString());
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreateTidexPostBody();
            body.Add("method", "orderInfo");
            body.Add("order_id", context.RemoteId);

            var r = await api.GetOrderInfoAsync(body).ConfigureAwait(false);

            CheckTidexResponse(r);

            if(r.return_.Count == 0 || !r.return_.TryGetValue(context.RemoteId, out var order))
                throw new NoTradeOrderException(context, this);

            return new TradeOrderStatus(context.RemoteId, order.status == 0, order.status == 2 || order.status == 3);
        }

        public decimal MinimumTradeVolume { get; }
    }
}
