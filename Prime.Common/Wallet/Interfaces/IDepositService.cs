using System;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IDepositService : IDescribesAssets
    {
        // BUG: consider removing.
        bool CanMultiDepositAddress { get; }

        bool CanGenerateDepositAddress { get; }

        bool CanPeekDepositAddress { get; }

        Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context);

        Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context);

        // Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context);
    }
}