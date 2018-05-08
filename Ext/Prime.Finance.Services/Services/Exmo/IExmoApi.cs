using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Exmo
{
    internal interface IExmoApi
    {
        [Get("/ticker")]
        Task<ExmoSchema.TickerResponse> GetTickersAsync();

        [Get("/currency")]
        Task<string[]> GetCurrencyAsync();

        [Get("/order_book/?pair={currencyPair}")]
        Task<ExmoSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
