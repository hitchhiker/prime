using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Gatecoin
{
    internal interface IGatecoinApi
    {
        [Get("/LiveTicker/{currencyPair}")]
        Task<GatecoinSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/LiveTickers")]
        Task<GatecoinSchema.TickersResponse> GetTickersAsync();

        [Get("/MarketDepth/{currencyPair}")]
        Task<GatecoinSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
