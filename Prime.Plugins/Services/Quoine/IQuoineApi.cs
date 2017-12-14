using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Quoine
{
    internal interface IQuoineApi
    {
        [Get("/products/{productId}")]
        Task<QuoineSchema.ProductResponse> GetProductAsync([Path] int productId);

        [Get("/products")]
        Task<QuoineSchema.ProductResponse[]> GetProductsAsync();
    }
}
