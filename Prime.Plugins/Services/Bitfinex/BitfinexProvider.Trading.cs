using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Bitfinex
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://bitfinex.readme.io/v1/reference
    public partial class BitfinexProvider : IOrderLimitProvider, IBalanceProvider
    {

        public Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public decimal MinimumTradeVolume { get; }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new BitfinexSchema.WalletBalancesRequest();

            var r = await api.GetWalletBalancesAsync(body).ConfigureAwait(false);

            var balances = new BalanceResults();

            foreach (var rBalance in r)
            {
                var asset = rBalance.currency.ToAsset(this);

                balances.Add(new BalanceResult(this)
                {
                    Available = new Money(rBalance.available, asset),
                    AvailableAndReserved = new Money(rBalance.amount, asset),
                    Reserved = new Money(0, asset)
                });
            }

            return balances;
        }
    }
}
