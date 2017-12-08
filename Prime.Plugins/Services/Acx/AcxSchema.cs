using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Acx
{
    internal class AcxSchema
    {
        internal class AllTickersResponse : Dictionary<string, TickersResponse>
        {
        }

        internal class TickerResponse
        {
            public long at;
            public TickerEntryResponse ticker;
        }

        internal class TickersResponse
        {
            public string name;
            public string base_unit;
            public string quote_unit;
            public long at;
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {
            public string fund_id;
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal open;
            public decimal close;
            public decimal volume;
            public decimal volume_traded;
            public decimal bid;
            public decimal ask;
            public DateTime date;
        }

        internal class MarketResponse
        {
            public string id;
            public string name;
            public string base_unit;
            public string quote_unit;
        }
    }
}
