using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Cex
{
    internal interface ICexApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<CexSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/tickers/USD/EUR/RUB/BTC")]
        Task<CexSchema.TickersResponse> GetTickersAsync();

        [Get("/last_price/{currencyPair}")]
        Task<CexSchema.LatestPriceResponse> GetLastPriceAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/last_prices/USD/EUR/RUB/BTC")]
        Task<CexSchema.LatestPricesResponse> GetLastPricesAsync();
    }
}
