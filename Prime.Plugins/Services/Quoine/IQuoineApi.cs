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

        [Post("/orders")]
        Task<Response<QuoineSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        //[Post("/orderHistory")]
        //Task<Response<QuoineSchema.OrderInfoResponse>> QueryOrdersAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        //[Post("/bitcoinWithdrawal")]
        //Task<Response<QuoineSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestBitcoinAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        //[Post("/litecoinWithdrawal")]
        //Task<Response<QuoineSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestLitecoinAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        //[Post("/bitcoinCashWithdrawal")]
        //Task<Response<QuoineSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestBitcoinCashAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

    }
}
