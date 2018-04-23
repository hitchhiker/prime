using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Gemini
{
    internal class GeminiSchema
    {
        internal class SymbolsResponse : List<string> { }

        internal class TickerResponse
        {
            public decimal ask;
            public decimal bid;
            public decimal last;
            public Dictionary<string, decimal> volume; // Is not needed at this point.
        }
    }
}
