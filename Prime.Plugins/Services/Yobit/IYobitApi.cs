using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Yobit
{
    internal interface IYobitApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<YobitSchema.AllTickersResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/info")]
        Task<YobitSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Get("/depth/{currencyPair}")]
        Task<YobitSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/")]
        Task<YobitSchema.UserInfoResponse> GetUserInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<YobitSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<YobitSchema.ActiveOrdersResponse>> QueryActiveOrdersAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<YobitSchema.OrderInfoResponse>> QueryOrderInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<YobitSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
