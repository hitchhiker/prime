using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coinfloor
{
    internal interface ICoinfloorApi
    {
        [Get("/{currencyPair}/ticker/")]
        Task<CoinfloorSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/{currencyPair}/order_book/")]
        Task<CoinfloorSchema.OrderBookResponse> GetOrderBookAsync([Path(UrlEncode = false)] string currencyPair);
    }
}
