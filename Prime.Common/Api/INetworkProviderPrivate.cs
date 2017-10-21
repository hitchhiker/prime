using System.Threading.Tasks;

namespace Prime.Common
{
    public interface INetworkProviderPrivate : INetworkProvider
    {
        ApiConfiguration GetApiConfiguration { get; }

        Task<bool> TestApiAsync(ApiTestContext context);
    }
}