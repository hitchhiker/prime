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
        Task<GeminiSchema.SymbolsResponse> GetSymbolsAsync();

        [Get("/pubticker/{currencyPair}")]
        Task<GeminiSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
