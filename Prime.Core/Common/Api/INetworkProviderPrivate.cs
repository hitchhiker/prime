using System.Threading.Tasks;

namespace Prime.Core
{
    public interface INetworkProviderPrivate : INetworkProvider
    {
        ApiConfiguration GetApiConfiguration { get; }

        // TODO: AY: change bool to PrivateApiResponse class.
        Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context);
    }
}