
using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IOhlcProvider : IDescribesAssets
    {
        Task<OhlcDataResponse> GetOhlcAsync(OhlcContext context);
    }
}