using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Common.Exchange;

namespace Prime.Common
{
    public interface IPublicPricesProvider : IPublicAssetPricesProvider
    {
        Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context);
    }
}