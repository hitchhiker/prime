using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Binance
{
    public partial class BinanceProvider : IBalanceProvider, IOrderLimitProvider
    {
        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountInformationAsync().ConfigureAwait(false);

            var balances = new BalanceResults(this);

            foreach (var b in r.balances)
            {
                var asset = b.asset.ToAsset(this);
                balances.Add(asset, b.free, b.locked);
            }

            return balances;
        }

        public Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public MinimumTradeVolume[] MinimumTradeVolume { get; }
    }
}
