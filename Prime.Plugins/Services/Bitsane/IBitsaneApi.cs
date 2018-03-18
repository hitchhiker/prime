using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitsane
{
    internal interface IBitsaneApi
    {
        [Get("/public/ticker?pairs={currencyPair}")]
        Task<BitsaneSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/public/ticker")]
        Task<BitsaneSchema.TickerResponse> GetTickersAsync();

        [Get("/public/orderbook?pair={currencyPair}")]
        Task<BitsaneSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/private/balances")]
        Task<Response<BitsaneSchema.BalanceResponse>> GetBalancesAsync();

        [Post("/private/withdraw")]
        Task<Response<BitsaneSchema.WithdrawalResponse>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/private/order/new")]
        Task<Response<BitsaneSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/private/order/status")]
        Task<Response<BitsaneSchema.OrderInfoResponse>> QueryOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

    }
}
