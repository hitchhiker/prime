using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.BlinkTrade
{
    [AllowAnyStatusCode]
    internal interface IBlinkTradeApi
    {
        [Get("/{fiat}/ticker?crypto_currency={crypto}")]
        Task<BlinkTradeSchema.TickerResponse> GetTickerAsync([Path] string fiat, [Path] string crypto);

        [Get("/{fiat}/orderbook?crypto_currency={crypto}")]
        Task<BlinkTradeSchema.OrderBookResponse> GetOrderBookAsync([Path] string fiat, [Path] string crypto);

        [Post("/")]
        Task<Response<BlinkTradeSchema.BalanceResponse>> GetBalanceAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<BlinkTradeSchema.ActiveOrdersResponse>> QueryActiveOrdersAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<BlinkTradeSchema.OrderInfoResponse>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<BlinkTradeSchema.WithdrawalResponse>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);
    }
}
