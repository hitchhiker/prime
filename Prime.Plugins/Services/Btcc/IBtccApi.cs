using RestEase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Btcc
{
    internal interface IBtccApi
    {
        [Get("/ticker/?symbol={currencyPair}")]
        Task<BtccSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
