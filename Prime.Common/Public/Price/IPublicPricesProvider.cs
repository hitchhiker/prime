using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPricesProvider : IPublicPriceProvider
    {
        Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context);
    }
}