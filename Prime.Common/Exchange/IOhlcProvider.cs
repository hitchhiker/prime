
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IOhlcProvider : IDescribesAssets
    {
        Task<OhlcDataResponse> GetOhlcAsync(OhlcContext context);
    }
}