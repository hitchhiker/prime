using System.Net.Http.Headers;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coinfloor
{
    [AllowAnyStatusCode]
    internal interface ICoinfloorApi
    {
        [Get("/{currencyPair}/ticker/")]
        Task<CoinfloorSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/{currencyPair}/order_book/")]
        Task<CoinfloorSchema.OrderBookResponse> GetOrderBookAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/{currencyPair}/balance/")]
        Task<Response<object>> GetBalancesAsync([Path(UrlEncode = false)] string currencyPair);
    }
}
