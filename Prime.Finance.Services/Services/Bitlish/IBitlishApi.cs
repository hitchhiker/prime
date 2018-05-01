using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Bitlish
{
    [AllowAnyStatusCode]
    internal interface IBitlishApi
    {
        [Get("/tickers")]
        Task<BitlishSchema.AllTickersResponse> GetTickersAsync();

        [Get("/trades_depth?pair_id={currencyPair}")]
        Task<BitlishSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/signin")]
        Task<Response<BitlishSchema.AuthenticationResponse>> AuthenticateUserAsync();

        [Get("/create_trade?pair_id={currencyPair}&dir={side}&amount={amount}&price={price}&authentication_token={authenticationToken}")]
        Task<Response<BitlishSchema.OrderResponse>> PlaceNewOrderAsync([Path(UrlEncode = false)] string authenticationToken, [Path] string currencyPair, [Path]string side, [Path]decimal amount, [Path]decimal price);

        [Get("/trade_details?id={orderId}&authentication_token={authenticationToken}")]
        Task<Response<BitlishSchema.OrderResponse>> QueryOrderAsync([Path(UrlEncode = false)] string authenticationToken, [Path] string orderId);
    }
}
