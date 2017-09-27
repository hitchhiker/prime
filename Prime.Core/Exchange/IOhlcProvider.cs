using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IOhlcProvider : INetworkProvider
    {
        Task<OhlcData> GetOhlcAsync(OhlcContext context);
    }
}