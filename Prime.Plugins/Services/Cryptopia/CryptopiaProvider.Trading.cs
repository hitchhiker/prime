using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;

namespace Prime.Plugins.Services.Cryptopia
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://www.cryptopia.co.nz/Forum/Thread/255
    public partial class CryptopiaProvider : IOrderLimitProvider, IBalanceProvider
    {
        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new CryptopiaSchema.SubmitTradeRequest();
            body.Market = context.Pair.ToTicker(this, "/");
            body.Type = context.IsBuy ? "Buy" : "Sell";
            body.Rate = context.Rate;
            body.Amount = context.Quantity;

            var rRaw = await api.SubmitTradeAsync(body).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();
            return new PlacedOrderLimitResponse(r.Data.OrderId.ToString());
        }

        private async Task<IEnumerable<TradeOrderStatus>> GetOpenOrdersAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new CryptopiaSchema.GetOpenOrdersRequest();
            body.Count = 1000;
            body.Market = context.Market.ToTicker(this, "/");

            var rRaw = await api.GetOpenOrdersAsync(body).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.Data.Select(x => new TradeOrderStatus(x.OrderId.ToString(), true, false));
        }

        private async Task<IEnumerable<TradeOrderStatus>> GetTradeHistoryAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new CryptopiaSchema.GetTradeHistoryRequest();
            body.Count = 1000;
            body.Market = context.Market.ToTicker(this, "/");

            var rRaw = await api.GetTradeHistoryAsync(body).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.Data.Select(x => new TradeOrderStatus(x.TradeId.ToString(), false, false));
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var openOrders = await GetOpenOrdersAsync(context).ConfigureAwait(false);

            var order = openOrders.FirstOrDefault(x => x.RemoteOrderId.Equals(context.RemoteId));

            var isOpen = true;

            if (order == null)
            {
                var tradeHistory = await GetTradeHistoryAsync(context).ConfigureAwait(false);

                var trade = tradeHistory.FirstOrDefault(x => x.RemoteOrderId.Equals(context.RemoteId));

                if(trade == null)
                    throw new NoTradeOrderException(context, this);

                isOpen = false;
            }

            return new TradeOrderStatus(context.RemoteId, isOpen, false);
        }

        public decimal MinimumTradeVolume { get; }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetBalanceAsync(new CryptopiaSchema.BalanceRequest()).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();
            
            var balances = new BalanceResults();

            foreach (var rBalance in r.Data)
            {
                var asset = rBalance.Symbol.ToAsset(this);

                balances.Add(new BalanceResult(this)
                {
                    Available = new Money(rBalance.Available, asset),
                    Reserved = new Money(rBalance.HeldForTrades + rBalance.PendingWithdraw, asset)
                });
            }

            return balances;
        }
    }
}
