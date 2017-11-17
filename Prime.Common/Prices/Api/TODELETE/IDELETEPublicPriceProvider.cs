using System;
using System.Threading.Tasks;

namespace Prime.Common
{
    [Obsolete]
    public interface IDELETEPublicPriceProvider : IPublicPricingProvider
    {
        Task<MarketPrice> GetPriceAsync(PublicPriceContext context);
    }
}