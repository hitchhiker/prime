using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.NLexch
{
    [AllowAnyStatusCode]
    internal interface INLexchApi
    {
        [Get("/tickers/{currencyPair}.json")]
        Task<NLexchSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/tickers.json")]
        Task<Dictionary<string, NLexchSchema.TickerResponse>> GetTickersAsync();

        [Get("/order_book.json?market={currencyPair}")]
        Task<NLexchSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/members/me.json")]
        Task<Response<NLexchSchema.UserInfoResponse>> GetUserInfoAsync();

        [Post("/orders.json")]
        Task<Response<NLexchSchema.OrderInfoResponse>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Get("/order.json?id={id}")]
        Task<Response<NLexchSchema.OrderInfoResponse>> QueryOrderAsync([Path] string id);
    }
}
