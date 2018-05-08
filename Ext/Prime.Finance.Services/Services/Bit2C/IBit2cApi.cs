using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Bit2C
{
    internal interface IBit2CApi
    {
        [Get("/Exchanges/{currencyPair}/Ticker.json")]
        Task<Bit2CSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/Exchanges/{currencyPair}/orderbook.json")]
        Task<Bit2CSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
