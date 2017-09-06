using System.Threading.Tasks;
using Prime.Core;
namespace Prime.Core
{
    public interface IExchangeProvider : IPublicPriceProvider
    {
        BuyResult Buy(BuyContext ctx);

        SellResult Sell(SellContext ctx);

        Task<AssetPairs> GetAssetPairs(NetworkProviderContext context);
    }
}