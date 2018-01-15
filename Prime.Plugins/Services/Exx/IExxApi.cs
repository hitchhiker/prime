using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Exx
{
    internal interface IExxApi
    {
        [Get("/ticker?currency={currencyPair}")]
        Task<ExxSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/tickers")]
        Task<ExxSchema.AllTickersResponse> GetTickersAsync();

        [Get("/depth?currency={currencyPair}")]
        Task<ExxSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
