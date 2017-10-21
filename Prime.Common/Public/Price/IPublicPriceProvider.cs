using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPriceProvider : INetworkProvider
    {
        Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context);
    }
}