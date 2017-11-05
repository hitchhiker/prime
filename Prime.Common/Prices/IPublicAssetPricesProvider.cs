using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicAssetPricesProvider : IPublicPriceSuper
    {
        Task<List<MarketPrice>> GetAssetPricesAsync(PublicAssetPricesContext context);
    }
}