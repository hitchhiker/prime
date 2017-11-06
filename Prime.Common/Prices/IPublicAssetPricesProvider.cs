using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Common.Exchange;

namespace Prime.Common
{
    public interface IPublicAssetPricesProvider : IPublicPriceSuper
    {
        Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context);
    }
}