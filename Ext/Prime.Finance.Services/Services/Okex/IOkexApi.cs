using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Okex
{
    internal interface IOkexApi
    {
        [Get("/ticker.do?symbol={currencyPair}")]
        Task<OkexSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/exchange_rate.do")]
        Task<OkexSchema.ExchangeRateResponse> GetExchangeRate();

        [Get("/depth.do?symbol={currencyPair}")]
        Task<OkexSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
