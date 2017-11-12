using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitMarket
{
    internal interface IBitMarketApi
    {
        [Get("/json/{currencyPair}/ticker.json")]
        Task<BitMarketSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
