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

        [Post("/info")]
        Task<YobitSchema.UserInfoResponse> GetUserInfoAsync();

        [Post("/Trade")]
        Task<Response<YobitSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/ActiveOrders")]
        Task<Response<YobitSchema.ActiveOrdersResponse>> QueryActiveOrdersAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/OrderInfo")]
        Task<Response<YobitSchema.OrderInfoResponse>> QueryOrderInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/WithdrawCoinsToAddress")]
        Task<Response<YobitSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
