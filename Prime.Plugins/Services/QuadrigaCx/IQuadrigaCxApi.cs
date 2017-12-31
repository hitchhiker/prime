using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.QuadrigaCx
{
    internal interface IQuadrigaCxApi
    {
        [Get("/ticker?book={currencyPair}")]
        Task<QuadrigaCxSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/order_book?book={currencyPair}")]
        Task<QuadrigaCxSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
