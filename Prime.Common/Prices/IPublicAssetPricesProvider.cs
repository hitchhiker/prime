using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicAssetPricesProvider : IPublicPrice
    {
        Task<List<LatestPrice>> GetAssetPricesAsync(PublicAssetPricesContext context);
    }
}