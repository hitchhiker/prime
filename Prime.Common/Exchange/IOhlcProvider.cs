
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IOhlcProvider : INetworkProvider
    {
        Task<OhlcData> GetOhlcAsync(OhlcContext context);
    }
}