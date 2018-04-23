using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Btcc
{
    internal interface IBtccApi
    {
        [Get("/ticker?symbol={currencyPair}")]
        Task<BtccSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/orderbook?symbol={currencyPair}&limit={limit}")]
        Task<BtccSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] int limit);
    }
}
