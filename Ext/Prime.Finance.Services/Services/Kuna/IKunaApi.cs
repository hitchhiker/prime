using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Kuna
{
    [AllowAnyStatusCode]
    internal interface IKunaApi
    {
        [Get("/tickers/{currencyPair}")]
        Task<KunaSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/tickers")]
        Task<KunaSchema.AllTickersResponse> GetTickersAsync();

        [Get("/timestamp")]
        Task<long> GetTimestamp();

        [Get("/order_book?market={currencyPair}")]
        Task<KunaSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/members/me")]
        Task<Response<KunaSchema.UserInfoResponse>> GetUserInfoAsync();

        [Post("/orders")]
        Task<Response<KunaSchema.OrderInfoResponse>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Get("/orders?market={currencyPair}")]
        Task<Response<KunaSchema.ActiveOrdersResponse>> QueryActiveOrdersAsync([Path] string currencyPair);
    }
}
