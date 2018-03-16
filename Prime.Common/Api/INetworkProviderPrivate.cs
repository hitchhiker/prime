using System.Threading.Tasks;

namespace Prime.Common
{
    public interface INetworkProviderPrivate : INetworkProvider
    {
        ApiConfiguration GetApiConfiguration { get; }

        // TODO: AY: change bool to PrivateApiResponse class.
        Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context);
    }
}