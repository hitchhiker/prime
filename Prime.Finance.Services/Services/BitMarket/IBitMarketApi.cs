using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.BitMarket
{
    internal interface IBitMarketApi
    {
        [Get("/json/{currencyPair}/ticker.json")]
        Task<BitMarketSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
