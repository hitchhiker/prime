using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPairsPricesProvider : INetworkProvider
    {
        Task<LatestPrice> GetPairsPricesAsync(PublicPairsPricesContext context);
    }
}