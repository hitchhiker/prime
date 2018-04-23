using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Bisq
{
    internal interface IBisqApi
    {
        [Get("/ticker?market={currencyPair}")]
        Task<BisqSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/ticker")]
        Task<BisqSchema.AllTickersResponse> GetAllTickers();

        [Get("/depth?market={currencyPair}")]
        Task<BisqSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
