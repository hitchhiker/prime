using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.NovaExchange
{
    internal interface INovaExchangeApi
    {
        [Get("/market/info/{currencyPair}/")]
        Task<NovaExchangeSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/markets/")]
        Task<NovaExchangeSchema.TickerResponse> GetTickersAsync();
    }
}
