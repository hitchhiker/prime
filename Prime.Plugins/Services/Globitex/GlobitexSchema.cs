using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Globitex
{
    internal class GlobitexSchema
    {
        internal class TimeResponse
        {
            public long timestamp;
        }

        internal class SymbolsResponse
        {
            public SymbolEntryResponse[] symbols;
        }

        internal class SymbolEntryResponse
        {
            public decimal priceIncrement;
            public decimal sizeIncrement;
            public decimal sizeMin;
            public string symbol;
            public string currency;
            public string commodity;
        }

        internal class AllTickersResponse
        {
            public TickerResponse[] instruments;
        }

        internal class TickerResponse
        {
            public string symbol;
            public decimal ask;
            public decimal bid;
            public decimal last;
            public decimal low;
            public decimal high;
            public decimal open;
            public decimal volume;
            public decimal volumeQuote;
            public long timestamp;
        }
    }
}
