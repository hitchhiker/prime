using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Okex
{
    internal interface IOkexApi
    {
        [Get("/ticker.do?symbol={currencyPair}")]
        Task<OkexSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
