using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Braziliex
{
    internal interface IBraziliexApi
    {
        [Get("/public/ticker/{currencyPair}")]
        Task<BraziliexSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/public/ticker")]
        Task<BraziliexSchema.AllTickersResponse> GetTickersAsync();
    }
}
