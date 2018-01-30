using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Kucoin
{

    [AllowAnyStatusCode]
    internal interface IKucoinApi
    {
        [Get("/open/tick?symbol={currencyPair}")]
        Task<KucoinSchema.BaseResponse<KucoinSchema.TickerResponse>> GetTickerAsync([Path] string currencyPair);

        [Get("/open/tick")]
        Task<KucoinSchema.BaseResponse<KucoinSchema.TickerResponse[]>> GetTickersAsync();

        [Get("/open/orders?symbol={currencyPair}&limit={limit}")]
        Task<KucoinSchema.BaseResponse<KucoinSchema.OrderBookResponse>> GetOrderBookAsync([Path] string currencyPair, [Path] int limit);

        [Get("/user/info")]
        Task<KucoinSchema.UserInfoResponse> GetUserInfoAsync();

        [Post("/order?symbol={symbol}")]
        Task<KucoinSchema.BaseResponse<KucoinSchema.NewOrderResponse>> NewOrderAsync([Path] string symbol, [Query] string type, [Query] decimal price, [Query] decimal amount);

        [Get("/order/active-map?symbol={symbol}")]
        Task<KucoinSchema.BaseResponse<KucoinSchema.QueryActiveOrdersResponse>> QueryActiveOrdersAsync([Path] string symbol);

        [Get("/deal-orders?symbol={symbol}")]
        Task<KucoinSchema.BaseResponse<KucoinSchema.QueryDealtOrdersResponse>> QueryDealtOrdersAsync([Path] string symbol);

        [Post("/account/coin/withdraw/apply")]
        Task<Response<KucoinSchema.BaseResponse<KucoinSchema.WithdrawalRequestResponse>>> SubmitWithdrawRequestAsync([Query] string coin, [Query] decimal amount, [Query] string address);

    }
}
