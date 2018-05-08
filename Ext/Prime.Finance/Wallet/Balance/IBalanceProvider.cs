using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Finance
{
    public interface IBalanceProvider : IDescribesAssets, INetworkProviderPrivate
    {
        Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context);
    }
}
