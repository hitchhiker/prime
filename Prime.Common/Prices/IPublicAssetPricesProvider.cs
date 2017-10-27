using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicAssetPricesProvider : INetworkProvider
    {
        Task<List<LatestPrice>> GetAssetPricesAsync(PublicAssetPricesContext context);
    }
}