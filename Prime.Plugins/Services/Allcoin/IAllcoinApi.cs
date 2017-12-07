using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Allcoin
{
    internal interface IAllcoinApi
    {
        [Get("/ticker?symbol={currencyPair}")]
        Task<AllcoinSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
