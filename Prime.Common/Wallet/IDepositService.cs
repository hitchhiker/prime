using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IDepositService : IDescribesAssets
    {
        bool CanMultiDepositAddress { get; }

        bool CanGenerateDepositAddress { get; }

        Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context);

        Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context);
    }
}