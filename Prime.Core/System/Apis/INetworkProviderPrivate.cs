using System.Threading.Tasks;

namespace Prime.Core
{
    public interface INetworkProviderPrivate : INetworkProvider
    {
        ApiConfiguration GetApiConfiguration { get; }

        Task<string> TestApi(ApiTestContext context);
    }
}