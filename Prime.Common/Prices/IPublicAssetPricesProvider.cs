using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicAssetPricesProvider : IPublicPairPriceProvider
    {
        Task<LatestPrices> GetAssetPricesAsync(PublicAssetPricesContext context);
    }
}