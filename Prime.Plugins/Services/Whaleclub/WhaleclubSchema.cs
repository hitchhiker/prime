using System.Collections;
using System.Collections.Generic;

namespace Prime.Plugins.Services.Whaleclub
{
    internal class WhaleclubSchema
    {
        internal class MarketsResponse : Dictionary<string, MarketResponse> { }
        
        internal class PricesResponse : Dictionary<string, PriceResponse> { }

        internal class MarketResponse
        {
            public string display_name;
            public string category;
        }

        internal class PriceResponse
        {
            public decimal bid;
            public decimal ask;
            public string state;
            public long last_updated;
        }
    }
}