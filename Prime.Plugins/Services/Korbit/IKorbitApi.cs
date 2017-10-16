using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Korbit
{
    internal interface IKorbitApi
    {
        [Get("/ticker?currency_pair={currencyPair}")]
        Task<KorbitSchema.TickerResponse> GetTicker([Path] string currencyPair);
    }
}
