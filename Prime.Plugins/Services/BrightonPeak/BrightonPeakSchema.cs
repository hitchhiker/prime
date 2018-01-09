using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BrightonPeak
{
    internal class BrightonPeakSchema
    {
        internal class AssetPairsResponse
        {
            public AssetPairsEntryResponse[] productPairs;
            public bool isAccepted;
        }

        internal class AssetPairsEntryResponse
        {
            public int productPairCode;
            public int product1DecimalPlaces;
            public int product2DecimalPlaces;
            public string name;
            public string product2Label;
            public string product1Label;
        }

        internal class TickerResponse
        {
            public int Total24HrNumTrades;
            public int sellOrderCount;
            public int buyOrderCount;
            public int numOfCreateOrders;
            public bool isAccepted;
            public decimal high;
            public decimal last;
            public decimal bid;
            public decimal volume;
            public decimal low;
            public decimal ask;
            public decimal Total24HrQtyTraded;
            public decimal Total24HrProduct2Traded;
        }

        internal class OrderBookResponse
        {
            public OrderBookItemResponse[] bids;
            public OrderBookItemResponse[] asks;
        }

        internal class OrderBookItemResponse
        {
            public decimal qty;
            public decimal px;
        }
    }
}
