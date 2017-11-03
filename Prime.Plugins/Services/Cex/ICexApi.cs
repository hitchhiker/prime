using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Cex
{
    internal interface ICexApi
    {
        [Get("/tickers/USD/EUR/RUB/BTC")]
        Task<CexSchema.TickersResponse> GetTickers();

        [Get("/last_price/{currencyPair}")]
        Task<CexSchema.LatestPriceResponse> GetLastPrice([Path(UrlEncode = false)] string currencyPair);

        [Get("/last_prices/USD/EUR/RUB/BTC")]
        Task<CexSchema.LatestPricesResponse> GetLastPrices();
    }
}
