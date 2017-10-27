using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Common
{
    public interface IExchangeProvider : IPublicPriceProvider, IAssetPairsProvider
    {
        BuyResult Buy(BuyContext ctx);

        SellResult Sell(SellContext ctx);
    }
}