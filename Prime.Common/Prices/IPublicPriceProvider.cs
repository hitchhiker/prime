using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPriceProvider : INetworkProvider
    {
        Task<LatestPrice> GetPriceAsync(PublicPriceContext context);
    }
}