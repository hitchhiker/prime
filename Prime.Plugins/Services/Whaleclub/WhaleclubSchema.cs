using System.Collections;
using System.Collections.Generic;

namespace Prime.Plugins.Services.Whaleclub
{
    internal class WhaleclubSchema
    {
        internal class MarketsResponse : Dictionary<string, MarketResponse> { }
        
        internal class MarketResponse
        {
            public string display_name;
            public string category;
        }
    }
}