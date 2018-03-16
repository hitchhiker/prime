using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Coinmate
{
    [AllowAnyStatusCode]
    internal interface ICoinmateApi
    {
        [Get("/ticker?currencyPair={currencyPair}")]
        Task<Response<CoinmateSchema.TickerResponse>> GetTickerAsync([Path] string currencyPair);

        [Get("/orderBook?currencyPair={currencyPair}&groupByPriceLimit={groupByPriceLimit}")]
        Task<Response<CoinmateSchema.OrderBookResponse>> GetOrderBookAsync([Path] string currencyPair, [Path] bool groupByPriceLimit);

        [Post("/balances")]
        Task<CoinmateSchema.BalanceResponse> GetBalanceAsync();

        [Post("/buyLimit")]
        Task<Response<CoinmateSchema.NewOrderResponse>> PlaceMarketBuyLimit([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/sellLimit")]
        Task<Response<CoinmateSchema.NewOrderResponse>> PlaceMarketSellLimit([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/orderHistory")]
        Task<Response<CoinmateSchema.OrderInfoResponse>> QueryOrdersAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/bitcoinWithdrawal")]
        Task<Response<CoinmateSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestBitcoinAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/litecoinWithdrawal")]
        Task<Response<CoinmateSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestLitecoinAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/bitcoinCashWithdrawal")]
        Task<Response<CoinmateSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestBitcoinCashAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
