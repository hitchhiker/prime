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
        Task<Dictionary<int, BxInThSchema.TickerResponse>> GetTickersAsync();

        [Get("/pairing/")]
        Task<Dictionary<int, BxInThSchema.CurrencyResponse>> GetCurrenciesAsync();
    }
}
