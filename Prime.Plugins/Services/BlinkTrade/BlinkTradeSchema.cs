using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BlinkTrade
{
    internal class BlinkTradeSchema
    {
        internal class TickerResponse : Dictionary<string, object> { }
        
        internal class OrderBookResponse
        {
            public string pair;
            public decimal[][] asks;
            public decimal[][] bids;
        }
    }
}
