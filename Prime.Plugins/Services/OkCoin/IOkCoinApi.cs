using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.OkCoin
{
    internal interface IOkCoinApi
    {
        [Get("/ticker.do?symbol={currencyPair}")]
        Task<OkCoinSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
