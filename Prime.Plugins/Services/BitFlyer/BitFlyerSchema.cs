using System;
using System.Collections.Generic;

namespace Prime.Plugins.Services.BitFlyer
{
    internal class BitFlyerSchema
    {
        internal class PricesResponse : List<PriceResponse> { }

        internal class MarketsResponse : List<MarketResponse> { }

        internal class BoardResponse
        {
            public decimal mid_price;
            public List<BidAskEntryResponse> bids;
            public List<BidAskEntryResponse> asks;
        }

        internal class BidAskEntryResponse
        {
            public decimal price;
            public decimal size;
        }

        internal class MarketResponse
        {
            public string product_code;
        }

        internal class TickerResponse
        {
            public string product_code;
            public DateTime timestamp;
            public long tick_id;
            public decimal best_bid;
            public decimal best_ask;
            public decimal best_bid_size;
            public decimal best_ask_size;
            public decimal total_bid_depth;
            public decimal total_ask_depth;
            public decimal ltp;
            public decimal volume;
            public decimal volume_by_product;
        }

        internal class PriceResponse
        {
            public string product_code;
            public string main_currency;
            public string sub_currency;
            public decimal rate;
        }
    }
}
