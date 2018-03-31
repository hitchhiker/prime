using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitso
{
    [AllowAnyStatusCode]
    internal interface IBitsoApi
    {
        [Get("/ticker?book={currencyPair}")]
        Task<BitsoSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/ticker/")]
        Task<BitsoSchema.AllTickerResponse> GetTickersAsync();

        [Get("/order_book?book={currencyPair}")]
        Task<BitsoSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/account_status")]
        Task<Response<BitsoSchema.UserInfoResponse>> GetUserInfoAsync();

        [Post("/orders")]
        Task<Response<BitsoSchema.OrderInfoResponse>> PlaceNewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Get("/orders/{oid}")]
        Task<Response<BitsoSchema.OrdersResponse>> QueryOrderAsync([Path]string oid);

        [Post("/bitcoin_withdrawal")]
        Task<Response<BitsoSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestBtcAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/ether_withdrawal")]
        Task<Response<BitsoSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestEthAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/ripple_withdrawal")]
        Task<Response<BitsoSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestXrpAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/bcash_withdrawal")]
        Task<Response<BitsoSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestBchAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/litecoin_withdrawal")]
        Task<Response<BitsoSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestLtcAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);
    }
}
