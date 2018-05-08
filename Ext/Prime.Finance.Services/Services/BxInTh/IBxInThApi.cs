using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.BxInTh
{
    internal interface IBxInThApi
    {
        [Get("/")]
        Task<BxInThSchema.TickersResponse> GetTickersAsync();

        [Get("/pairing/")]
        Task<BxInThSchema.CurrenciesResponse> GetCurrenciesAsync();
    }
}
