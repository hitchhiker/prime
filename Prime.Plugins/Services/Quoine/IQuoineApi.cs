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
        Task<Response<QuoineSchema.OrderInfoResponse>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Get("/orders/{id}")]
        Task<Response<QuoineSchema.OrderInfoResponse>> QueryOrdersAsync([Path] string id);
    }
}
