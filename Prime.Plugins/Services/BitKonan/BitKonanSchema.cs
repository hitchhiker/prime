using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BitKonan
{
    internal class BitKonanSchema
    {
        internal class TickerResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal bid;
            public decimal ask;
            public decimal open;
            public decimal volume;
        }

        internal class OrderBookEntryBtcResponse
        {
            public decimal usd;
            public decimal btc;
        }

        internal class OrderBookEntryLtcResponse
        {
            public decimal usd;
            public decimal ltc;
        }

        internal class OrderBookBtcResponse
        {
            public OrderBookEntryBtcResponse[] bid;
            public OrderBookEntryBtcResponse[] ask;
        }

        internal class OrderBookLtcResponse
        {
            public OrderBookEntryLtcResponse[] bid;
            public OrderBookEntryLtcResponse[] ask;
        }
    }
}
