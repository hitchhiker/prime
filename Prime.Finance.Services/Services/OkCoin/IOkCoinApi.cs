using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.OkCoin
{
    internal interface IOkCoinApi
    {
        [Get("/ticker.do?symbol={currencyPair}")]
        Task<OkCoinSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/depth.do?symbol={currencyPair}")]
        Task<OkCoinSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
