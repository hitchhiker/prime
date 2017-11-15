using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Btcc
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
    }
}
