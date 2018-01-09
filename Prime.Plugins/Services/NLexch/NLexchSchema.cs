using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.NLexch
{
    internal class NLexchSchema
    {
        internal class TickerResponse
        {
            public long at;
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {
            public decimal buy;
            public decimal sell;
            public decimal low;
            public decimal high;
            public decimal last;
            public decimal vol;
        }

        internal class OrderBookItemResponse
        {
            public int id;
            public int trades_count;
            public decimal price;
            public decimal avg_price;
            public decimal volume;
            public decimal remaining_volume;
            public decimal executed_volume;
            public string side;
            public string ord_type;
            public string state;
            public string market;
            public DateTime created_at;
        }

        internal class OrderBookResponse
        {
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }
    }
}
