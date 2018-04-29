using System.Threading.Tasks;

namespace Prime.Core
{
    public interface ICreateDepositAddress
    {

        Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context);
    }
}