using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Coinroom
{
    internal class CoinroomSchema
    {
        internal class TickerResponse
        {
            public decimal low;
            public decimal high;
            public decimal vwap;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public decimal last;
        }

        internal class CurrenciesResponse
        {
            public string[] real;
            public string[] crypto;
        }
    }
}
