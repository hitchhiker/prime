using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IWalletService : INetworkProvider, IDescribesAssets, INetworkProviderPrivate
    {
        bool CanMultiDepositAddress { get; }

        bool CanGenerateDepositAddress { get; }

        Task<WalletAddresses> FetchDepositAddressesAsync(WalletAddressAssetContext context);

        Task<WalletAddresses> FetchAllDepositAddressesAsync(WalletAddressContext context);

        Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context);
    }
}
