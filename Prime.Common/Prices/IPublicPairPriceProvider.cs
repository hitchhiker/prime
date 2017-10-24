using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPairPriceProvider : INetworkProvider
    {
        Task<LatestPrice> GetPairPriceAsync(PublicPairPriceContext context);
    }
}