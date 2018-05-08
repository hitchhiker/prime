using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.TheRockTrading
{
    internal interface ITheRockTradingApi
    {
        [Get("/funds/{currencyPair}/ticker")]
        Task<TheRockTradingSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/funds/tickers")]
        Task<TheRockTradingSchema.AllTickersResponse> GetTickersAsync();

        [Get("/funds/{currencyPair}/orderbook")]
        Task<TheRockTradingSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
