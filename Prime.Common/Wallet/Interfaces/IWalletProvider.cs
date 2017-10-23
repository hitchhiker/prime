using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IWalletService : IDepositService, IDescribesAssets, INetworkProviderPrivate
    {
        Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context);
    }
}
