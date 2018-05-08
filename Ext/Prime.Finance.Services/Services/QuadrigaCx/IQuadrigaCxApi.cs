using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.QuadrigaCx
{
    internal interface IQuadrigaCxApi
    {
        [Get("/ticker?book={currencyPair}")]
        Task<QuadrigaCxSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/order_book?book={currencyPair}")]
        Task<QuadrigaCxSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
