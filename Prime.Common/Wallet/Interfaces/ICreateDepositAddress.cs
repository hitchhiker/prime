using System.Threading.Tasks;

namespace Prime.Common
{
    public interface ICreateDepositAddress
    {

        Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context);
    }
}