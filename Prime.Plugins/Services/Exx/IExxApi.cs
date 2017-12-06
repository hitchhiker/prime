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
        Task<Dictionary<string, ExxSchema.TickerEntry>> GetTickersAsync();
    }
}
