using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IPublicPricesProvider : IPublicPriceProvider
    {
        Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context);
    }
}