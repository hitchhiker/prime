using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bitsane
{
    internal class BitsaneSchema
    {
        internal class TickerResponse : Dictionary<string, TickerEntryResponse>
        {
        }

        internal class TickerEntryResponse
        {
            public decimal last;
            public decimal lowestAsk;
            public decimal highestBid;
            public decimal percentChange;
            public decimal baseVolume;
            public decimal quoteVolume;
            public decimal high24hr;
            public decimal low24hr;
            public decimal euroEquivalent;
            public decimal bitcoinEquivalent;
        }

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] bids;
            public OrderBookItemResponse[] asks;
        }

        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal amount;
            public long timestamp;
        }

        internal class OrderBookResponse
        {
            public int statusCode;
            public string statusText;
            public OrderBookEntryResponse result;
        }
    }
}
