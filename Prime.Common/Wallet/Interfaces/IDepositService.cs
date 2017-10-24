using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IDepositService : IDescribesAssets
    {
        bool CanMultiDepositAddress { get; }

        bool CanGenerateDepositAddress { get; }

        // TOTO: implement.
        // bool CanPeekDepositAddress { get; }

        Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context);

        Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context);
    }
}