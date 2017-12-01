using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BxInTh
{
    internal interface IBxInThApi
    {
        [Get("/")]
        Task<BxInThSchema.TickersResponse> GetTickersAsync();

        [Get("/pairing/")]
        Task<BxInThSchema.CurrenciesResponse> GetCurrenciesAsync();
    }
}
