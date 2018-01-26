using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Kucoin
{
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
    }
}
