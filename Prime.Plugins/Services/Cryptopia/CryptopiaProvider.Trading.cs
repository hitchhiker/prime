using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

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

        public Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
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
