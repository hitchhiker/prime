using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.LiveCoin
{
    internal interface ILiveCoinApi
    {
        [Get("/ticker?currencyPair={currencyPair}")]
        Task<LiveCoinSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/ticker")]
        Task<LiveCoinSchema.TickerResponse[]> GetTickersAsync();

        [Get("/order_book?currencyPair={currencyPair}&depth={depth}")]
        Task<LiveCoinSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] int depth);
    }
}
