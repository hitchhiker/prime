using System;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Finance
{
    public interface IDepositProvider : IDescribesAssets
    {
        bool CanGenerateDepositAddress { get; }

        bool CanPeekDepositAddress { get; }

        Task<WalletAddressesResult> GetAddressesForAssetAsync(WalletAddressAssetContext context);

        Task<WalletAddressesResult> GetAddressesAsync(WalletAddressContext context);

        Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context);
    }
}