
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IOhlcProvider : IDescribesAssets
    {
        Task<OhlcData> GetOhlcAsync(OhlcContext context);
    }
}