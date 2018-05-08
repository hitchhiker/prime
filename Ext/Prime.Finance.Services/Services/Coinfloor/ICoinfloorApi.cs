using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coinfloor
{
    [AllowAnyStatusCode]
    [Header("Content-Type", "application/x-www-form-urlencoded")]
    internal interface ICoinfloorApi
    {
        [Get("/{currencyPair}/ticker/")]
        Task<CoinfloorSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/{currencyPair}/order_book/")]
        Task<CoinfloorSchema.OrderBookResponse> GetOrderBookAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/{currencyPair}/balance/")]
        Task<Response<CoinfloorSchema.BalancesResponse>> GetBalancesAsync([Path(UrlEncode = false)] string currencyPair);

        [Post("/{currencyPair}/{side}/")]
        Task<Response<CoinfloorSchema.OrderResponse>> NewOrderAsync([Path(UrlEncode = false)] string currencyPair, [Path(UrlEncode = false)] string side, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Get("/{currencyPair}/open_orders/")]
        Task<Response<CoinfloorSchema.OrderResponse[]>> QueryActiveOrdersAsync([Path(UrlEncode = false)] string currencyPair);
    }
}
