using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Globitex
{
    internal interface IGlobitexApi
    {
        [Get("/public/ticker/{currencyPair}")]
        Task<GlobitexSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/public/ticker")]
        Task<GlobitexSchema.AllTickersResponse> GetTickersAsync();

        [Get("/public/symbols")]
        Task<GlobitexSchema.SymbolsResponse> GetSymbolsAsync();

        [Get("/public/time")]
        Task<GlobitexSchema.TimeResponse> GetTimeAsync();
    }
}
