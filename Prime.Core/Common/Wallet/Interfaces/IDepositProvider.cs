using System;
using System.Threading.Tasks;

namespace Prime.Core
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