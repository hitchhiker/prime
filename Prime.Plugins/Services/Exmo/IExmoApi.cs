using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using RestEase;

namespace Prime.Plugins.Services.Exmo
{
    internal interface IExmoApi
    {
        [Get("/ticker")]
        Task<ExmoSchema.TickerResponse> GetTickersAsync();

        [Get("/currency")]
        Task<string[]> GetCurrencyAsync();
    }
}
