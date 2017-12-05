using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Alphapoint
{
    internal class AlphapointSchema
    {
        internal class ProductPairsResponse
        {
            public ProductPairEntry[] productPairs;
        }

        internal class ProductPairEntry
        {
            public int productPairCode;
            public int product1DecimalPlaces;
            public int product2DecimalPlaces;
            public string name;
            public string product1Label;
            public string product2Label;
        }

        internal class TickerResponse
        {
            public int Total24HrNumTrades;
            public int sellOrderCount;
            public int buyOrderCount;
            public int numOfCreateOrder;
            public decimal high;
            public decimal last;
            public decimal bid;
            public decimal volume;
            public decimal low;
            public decimal ask;
            public decimal Total24HrQtyTraded;
            public decimal Total24HrProduct2Traded;
            public bool isAccepted;
        }
    }
}
