using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IOhlcProvider : INetworkProvider
    {
        Task<OhclData> GetOhlcAsync(OhlcContext context);
    }
}