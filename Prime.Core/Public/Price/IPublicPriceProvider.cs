using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IPublicPriceProvider : INetworkProvider
    {
        Task<Money> GetLastPrice(PublicPriceContext context);
    }
}