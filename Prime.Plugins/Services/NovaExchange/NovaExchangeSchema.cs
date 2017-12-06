using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.NovaExchange
{
    internal class NovaExchangeSchema
    {
        internal class TickerResponse
        {
            public string status;
            public string message;
            public TickerEntryResponse[] markets;
        }

        internal class TickerEntryResponse
        {
            public int marketid;
            public int disabled;
            public string currency;
            public string basecurrency;
            public string marketname;
            public decimal low24h;
            public decimal high24h;
            public decimal ask;
            public decimal bid;
            public decimal change24h;
            public decimal last_price;
            public decimal volume24h;
        }
    }
}
