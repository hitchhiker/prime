using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coinsecure
{
    internal interface ICoinsecureApi
    {
        [Get("/exchange/ticker")]
        Task<CoinsecureSchema.TickerResponse> GetTickersAsync();
    }
}
