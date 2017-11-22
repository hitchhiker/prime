using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Gatecoin
{
    internal interface IGatecoinApi
    {
        [Get("/LiveTicker/{currencyPair}")]
        Task<GatecoinSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/LiveTickers")]
        Task<GatecoinSchema.TickersResponse> GetTickersAsync();
    }
}
