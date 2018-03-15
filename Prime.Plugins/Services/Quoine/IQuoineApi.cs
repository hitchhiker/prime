using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Quoine
{
    [AllowAnyStatusCode]
    internal interface IQuoineApi
    {
        [Get("/products/{productId}")]
        Task<QuoineSchema.ProductResponse> GetProductAsync([Path] int productId);

        [Get("/products")]
        Task<QuoineSchema.ProductResponse[]> GetProductsAsync();

        [Get("/products/{productId}/price_levels")]
        Task<QuoineSchema.OrderBookResponse> GetOrderBookAsync([Path] int productId);

        [Get("/accounts/balance")]
        Task<Response<QuoineSchema.AccountBalanceResponse[]>> GetBalancesAsync();

        //[Post("/buyLimit")]
        //Task<Response<CoinmateSchema.NewOrderResponse>> PlaceMarketBuyLimit([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        //[Post("/sellLimit")]
        //Task<Response<CoinmateSchema.NewOrderResponse>> PlaceMarketSellLimit([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        //[Post("/orderHistory")]
        //Task<Response<CoinmateSchema.OrderInfoResponse>> QueryOrdersAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        //[Post("/bitcoinWithdrawal")]
        //Task<Response<CoinmateSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestBitcoinAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        //[Post("/litecoinWithdrawal")]
        //Task<Response<CoinmateSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestLitecoinAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        //[Post("/bitcoinCashWithdrawal")]
        //Task<Response<CoinmateSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestBitcoinCashAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

    }
}
