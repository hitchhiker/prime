using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Exmo
{
    internal interface IExmoApi
    {
        [Get("/ticker")]
        Task<ExmoSchema.TickerResponse> GetTickersAsync();

        [Get("/currency")]
        Task<string[]> GetCurrencyAsync();

        [Get("/order_book/?pair={currencyPair}")]
        Task<ExmoSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/user_info")]
        Task<ExmoSchema.UserInfoResponse> GetUserInfoAsync();

        [Post("/order_create")]
        Task<Response<ExmoSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/user_open_orders")]
        Task<Response<ExmoSchema.ActiveOrdersResponse>> QueryActiveOrdersAsync();

        [Post("/withdraw_crypt")]
        Task<Response<ExmoSchema.WithdrawalResponse>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
