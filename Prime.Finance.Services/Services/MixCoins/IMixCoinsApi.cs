using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.MixCoins
{
    internal interface IMixCoinsApi
    {
        [Get("/ticker?market={currencyPair}")]
        Task<MixCoinsSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/depth?market={currencyPair}")]
        Task<MixCoinsSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
