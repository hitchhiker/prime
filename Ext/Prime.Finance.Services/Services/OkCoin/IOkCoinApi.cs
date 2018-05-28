using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.OkCoin
{
    [AllowAnyStatusCode]
    internal interface IOkCoinApi
    {
        [Get("/ticker.do?symbol={currencyPair}")]
        Task<OkCoinSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/depth.do?symbol={currencyPair}")]
        Task<OkCoinSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/userinfo.do")]
        Task<Response<OkCoinSchema.UserInfoResponse>> GetUserInfoAsync();

        [Post("/trade.do")]
        Task<Response<OkCoinSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/order_info.do")]
        Task<Response<OkCoinSchema.OrderResponse>> QueryOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
