﻿namespace Prime.Finance.Services.Services.Btcc
{
    internal class BtccSchema
    {
        internal class TickerResponse
        {
            public TickerEntryResponse ticker;
          
        }

        internal class TickerEntryResponse
        {
            public decimal Last;
            public decimal LastQuantity;
            public decimal PrevCls;
            public decimal High;
            public decimal Low;
            public decimal Open;
            public decimal Volume;
            public decimal Volume24H;
            public decimal ExecutionLimitDown;
            public decimal BidPrice;
            public decimal AskPrice;
            public decimal ExecutionLimitUp;
            public long Timestamp;
        }

        internal class OrderBookResponse
        {
            public long date;
            public decimal[][] bids;
            public decimal[][] asks;
        }
    }
}
