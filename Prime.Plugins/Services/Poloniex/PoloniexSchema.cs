﻿using System.Collections.Generic;

namespace Prime.Plugins.Services.Poloniex
{
    internal class PoloniexSchema
    {
        internal class BalancesDetailedResponse : Dictionary<string, BalanceDetailedResponse> { }

        internal class TickerResponse : Dictionary<string, TickerEntryResponse> { }

        internal class DepositAddressesResponse : Dictionary<string, string> { }

        internal class ChartEntriesResponse : List<ChartEntryResponse> { }

        internal class VolumeResponse : Dictionary<string, object> { }

        internal class OrderBookResponse
        {
            public decimal[][] asks;
            public decimal[][] bids;
            public int isFrozen;
            public int seq;
        }

        internal class ChartEntryResponse
        {
            public long date;
            public decimal high;
            public decimal low;
            public decimal open;
            public decimal close;
            public decimal volume;
            public decimal quoteVolume;
            public decimal weightedAverage;
        }

        internal class TickerEntryResponse
        {
            public int id;
            public decimal last;
            public decimal lowestAsk;
            public decimal highestBid;
            public decimal percentChange;
            public decimal baseVolume;
            public decimal quoteVolume;
            public string isFrozen;
            public decimal high24hr;
            public decimal low24hr;
        }

        internal class BalanceDetailedResponse
        {
            public decimal available;
            public decimal onOrders;
            public decimal btcValue;
        }
    }
}
