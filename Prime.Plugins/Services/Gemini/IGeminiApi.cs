using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Gemini
{
    internal interface IGeminiApi
    {
        [Get("/symbols")]
        Task<GeminiSchema.SymbolsResponse> GetSymbols();

        [Get("/pubticker/{currencyPair}")]
        Task<GeminiSchema.TickerResponse> GetTicker([Path] string currencyPair);
    }
}
