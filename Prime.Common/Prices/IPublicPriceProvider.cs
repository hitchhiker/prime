using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPriceProvider : IPublicPriceSuper
    {
        Task<MarketPrice> GetPriceAsync(PublicPriceContext context);
    }
}