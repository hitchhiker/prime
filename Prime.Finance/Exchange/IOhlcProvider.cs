
using System.Threading.Tasks;

namespace Prime.Finance
{
    public interface IOhlcProvider : IDescribesAssets
    {
        Task<OhlcDataResponse> GetOhlcAsync(OhlcContext context);
    }
}