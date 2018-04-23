using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.MercadoBitcoin
{
    internal interface IMercadoBitcoinApi
    {
        [Get("/{baseAsset}/ticker")]
        Task<MercadoBitcoinSchema.TickerResponse> GetTickerAsync([Path] string baseAsset);

        [Get("/{baseAsset}/orderbook")]
        Task<MercadoBitcoinSchema.OrderBookResponse> GetOrderBookAsync([Path] string baseAsset);
    }
}
