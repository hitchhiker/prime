using System.Net.Http.Headers;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coinfloor
{
    [Header("Accept", "*/*")]
    [Header("Content-Type", "application/x-www-form-urlencoded")]
    internal interface ICoinfloorApi
    {
        [Get("/{currencyPair}/ticker/")]
        Task<CoinfloorSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/{currencyPair}/order_book/")]
        Task<CoinfloorSchema.OrderBookResponse> GetOrderBookAsync([Path(UrlEncode = false)] string currencyPair);

        [Post("{currencyPair}/balance")]
        Task<object> GetBalancesAsync([Path(UrlEncode = false)] string currencyPair);
    }
}
