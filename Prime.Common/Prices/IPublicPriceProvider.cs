using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPriceProvider : IPublicPriceSuper
    {
        Task<LatestPrice> GetPriceAsync(PublicPriceContext context);
    }
}