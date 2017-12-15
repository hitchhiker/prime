using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitlish
{
    internal interface IBitlishApi
    {
        [Get("/tickers")]
        Task<BitlishSchema.AllTickersResponse> GetTickersAsync();
    }
}
