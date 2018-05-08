using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coincheck
{
    internal interface ICoincheckApi
    {
        [Get("/ticker/?pair={currencyPair}")]
        Task<CoincheckSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/order_books")]
        Task<CoincheckSchema.OrderBookResponse> GetOrderBookAsync();
    }
}
