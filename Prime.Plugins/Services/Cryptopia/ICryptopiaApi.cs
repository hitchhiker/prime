using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Cryptopia
{
    internal interface ICryptopiaApi
    {
        [Get("/GetMarket/{assetPair}")]
        Task<CryptopiaSchema.TickerResponse> GetTickerAsync([Path] string assetPair);

        [Get("/GetMarkets")]
        Task<CryptopiaSchema.AllTickersResponse> GetTickersAsync();

        [AllowAnyStatusCode]
        [Post("/GetBalance")]
        Task<Response<CryptopiaSchema.BalanceResponse>> GetBalanceAsync([Body(BodySerializationMethod.Serialized)] CryptopiaSchema.BalanceRequest body);

        [Post("/SubmitTrade")]
        Task<Response<CryptopiaSchema.SubmitTradeResponse>> SubmitTradeAsync([Body(BodySerializationMethod.Serialized)] CryptopiaSchema.SubmitTradeRequest body);

        [Post("/SubmitWithdraw")]
        Task<Response<CryptopiaSchema.WithdrawTradeResponse>> SubmitWithdrawAsync([Body(BodySerializationMethod.Serialized)] CryptopiaSchema.SubmitTradeRequest body);

        [Post("/GetOpenOrders")]
        Task<Response<CryptopiaSchema.OpenOrdersResponse>> GetOpenOrdersAsync([Body(BodySerializationMethod.Serialized)] CryptopiaSchema.GetOpenOrdersRequest body);

        [Post("/GetTradeHistory")]
        Task<Response<CryptopiaSchema.TradeHistoryResponse>> GetTradeHistoryAsync([Body(BodySerializationMethod.Serialized)] CryptopiaSchema.GetTradeHistoryRequest body);
    }
}
