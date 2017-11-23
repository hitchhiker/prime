using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bisq
{
    internal interface IBisqApi
    {
        [Get("/ticker?market={currencyPair}")]
        Task<List<BisqSchema.TickerResponseEntry>> GetTickerAsync([Path] string currencyPair);

        [Get("/ticker")]
        Task<Dictionary<string, BisqSchema.TickerResponseEntry>> GetAllTickers();
    }
}
