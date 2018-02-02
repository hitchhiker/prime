using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitBay
{
    [AllowAnyStatusCode]
    internal interface IBitBayApi
    {
        [Get("/Public/{currencyPair}/ticker.json")]
        Task<BitBaySchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/Public/{currencyPair}/orderbook.json")]
        Task<BitBaySchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/Trading/tradingApi.php")]
        Task<BitBaySchema.UserInfoResponse> GetUserInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/Trading/tradingApi.php")]
        Task<Response<BitBaySchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/Trading/tradingApi.php")]
        Task<Response<BitBaySchema.OrdersResponse[]>> QueryOrdersAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
        
        [Post("/Trading/tradingApi.php")]
        Task<Response<BitBaySchema.WithdrawalRequestResponse>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

    }
}
