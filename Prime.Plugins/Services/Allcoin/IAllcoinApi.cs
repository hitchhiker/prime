using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Allcoin
{
    internal interface IAllcoinApi
    {
        [Get("/ticker?symbol={currencyPair}")]
        Task<AllcoinSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/depth?symbol={currencyPair}")]
        Task<AllcoinSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/userinfo")]
        Task<Response<AllcoinSchema.UserInfoResponse>> GetUserInfoAsync();

        [Post("/trade")]
        Task<Response<AllcoinSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/order_info")]
        Task<Response<AllcoinSchema.OrderInfoResponse>> GetOrderInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
