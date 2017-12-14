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
        public Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public decimal MinimumTradeVolume { get; }
        public Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            throw new NotImplementedException();
        }
    }
}
