using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Gemini
{
    internal interface IGeminiApi
    {
        [Get("/symbols")]
        Task<GeminiSchema.SymbolsResponse> GetSymbolsAsync();

        [Get("/pubticker/{currencyPair}")]
        Task<GeminiSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
