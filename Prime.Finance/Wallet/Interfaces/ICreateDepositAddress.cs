using System.Threading.Tasks;

namespace Prime.Finance
{
    public interface ICreateDepositAddress
    {

        Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context);
    }
}