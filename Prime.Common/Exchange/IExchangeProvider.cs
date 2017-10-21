using System.Threading.Tasks;
using Prime.Common;
namespace Prime.Common
{
    public interface IExchangeProvider : IPublicPricesProvider
    {
        BuyResult Buy(BuyContext ctx);

        SellResult Sell(SellContext ctx);

        Task<AssetPairs> GetAssetPairs(NetworkProviderContext context);
    }
}