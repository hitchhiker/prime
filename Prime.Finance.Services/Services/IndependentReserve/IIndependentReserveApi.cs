using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.IndependentReserve
{
    internal interface IIndependentReserveApi
    {
        [Get("/GetMarketSummary?primaryCurrencyCode={primary}&secondaryCurrencyCode={secondary}")]
        Task<IndependentReserveSchema.TickerResponse> GetTickerAsync([Path] string primary, [Path]string secondary);

        [Get("/GetOrderBook?primaryCurrencyCode={primary}&secondaryCurrencyCode={secondary}")]
        Task<IndependentReserveSchema.OrderBookResponse> GetOrderBookAsync([Path] string primary, [Path]string secondary);
    }
}
