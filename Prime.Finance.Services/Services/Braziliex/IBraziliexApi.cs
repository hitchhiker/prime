using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Braziliex
{
    internal interface IBraziliexApi
    {
        [Get("/public/ticker/{currencyPair}")]
        Task<BraziliexSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/public/ticker")]
        Task<BraziliexSchema.AllTickersResponse> GetTickersAsync();

        [Get("/public/orderbook/{currencyPair}")]
        Task<BraziliexSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
