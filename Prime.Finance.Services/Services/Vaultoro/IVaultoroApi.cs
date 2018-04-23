using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Vaultoro
{
    internal interface IVaultoroApi
    {
        [Get("/markets")]
        Task<VaultoroSchema.MarketResponse> GetMarketsAsync();

        [Get("/orderbook")]
        Task<VaultoroSchema.OrderBookResponse> GetOrderBookAsync();
    }
}
