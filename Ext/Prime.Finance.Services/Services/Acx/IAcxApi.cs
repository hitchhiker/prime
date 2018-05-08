using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Acx
{
    [AllowAnyStatusCode]
    internal interface IAcxApi
    {
        [Get("/tickers/{currencyPair}.json")]
        Task<AcxSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/tickers.json")]
        Task<AcxSchema.AllTickersResponse> GetTickersAsync();

        [Get("/markets.json")]
        Task<AcxSchema.MarketResponse[]> GetAssetPairsAsync();

        [Get("/depth.json?market={currencyPair}")]
        Task<AcxSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/members/me.json")]
        Task<Response<AcxSchema.UserInfoResponse>> GetUserInfoAsync();

        [Post("/orders.json")]
        Task<Response<AcxSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Get("/order.json")]
        Task<Response<AcxSchema.OrderInfoResponse>> GetOrderInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        //TODO: SC - See what object endpoint returns, as there is no documentation to demonstrate this, so must be tested with real money.
        [Post("/withdraw.json")]
        Task<Response<object>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
