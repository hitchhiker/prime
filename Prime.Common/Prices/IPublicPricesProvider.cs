using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPricesProvider : IPublicAssetPricesProvider
    {
        Task<List<LatestPrice>> GetPricesAsync(PublicPricesContext context);
    }
}