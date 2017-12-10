using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Coinsecure
{
    internal interface ICoinsecureApi
    {
        [Get("/exchange/ticker")]
        Task<CoinsecureSchema.TickerResponse> GetTickersAsync();
    }
}
