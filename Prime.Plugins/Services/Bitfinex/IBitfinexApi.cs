using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitfinex
{
    [AllowAnyStatusCode]
    internal interface IBitfinexApi
    {
        [Get("/pubticker/{currencyPair}")]
        Task<Response<BitfinexSchema.TickerResponse>> GetTickerAsync([Path] string currencyPair);

        [Get("/symbols")]
        Task<Response<string[]>> GetAssetsAsync();

        [Get("/book/{currencyPair}")]
        Task<Response<BitfinexSchema.OrderBookResponse>> GetOrderBookAsync([Path] string currencyPair);

        [Post("/account_infos")]
        Task<Response<BitfinexSchema.AccountInfoResponse>> GetAccountInfoAsync([Body(BodySerializationMethod.Serialized)] object body);

        [Post("/balances")]
        Task<Response<BitfinexSchema.WalletBalanceResponse>> GetWalletBalancesAsync([Body(BodySerializationMethod.Serialized)] object body);

        [Post("/order/new")]
        Task<Response<BitfinexSchema.NewOrderResponse>> PlaceNewOrderAsync([Body(BodySerializationMethod.Serialized)] object body);

        [Post("/withdraw")]
        Task<Response<BitfinexSchema.WithdrawalsResponse>> WithdrawAsync([Body(BodySerializationMethod.Serialized)] object body);

        [Post("/order/status")]
        Task<Response<BitfinexSchema.OrderStatusResponse>> GetOrderStatusAsync([Body(BodySerializationMethod.Serialized)] object body);

        [Post("/orders")]
        Task<Response<BitfinexSchema.ActiveOrdersResponse>> GetActiveOrdersAsync([Body(BodySerializationMethod.Serialized)] object body);

        [Post("/orders/hist")]
        Task<Response<BitfinexSchema.OrdersHistoryResponse>> GetOrdersHistoryAsync([Body(BodySerializationMethod.Serialized)] object body);
    }
}
