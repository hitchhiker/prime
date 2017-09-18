using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IPublicPriceProvider : INetworkProvider
    {
        Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context);
        // Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context);
    }
}