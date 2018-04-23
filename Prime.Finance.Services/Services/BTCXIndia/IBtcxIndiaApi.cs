using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.BTCXIndia
{
    internal interface IBtcxIndiaApi
    {
        [Get("/ticker")]
        Task<BtcxIndiaSchema.TickerResponse> GetTickersAsync();
    }
}
