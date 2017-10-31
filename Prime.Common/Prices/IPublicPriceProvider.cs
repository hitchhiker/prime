using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPriceProvider : IPublicPrice
    {
        Task<LatestPrice> GetPriceAsync(PublicPriceContext context);
    }
}