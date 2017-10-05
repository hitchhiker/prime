using System;
using Prime.Core.Exchange;

namespace Prime.Core
{
    public class OrderBookRecord : IEquatable<OrderBookRecord>
    {
        public OrderBookRecord()
        {
            
        }

        public OrderBookRecord(BidAskData bidData, BidAskData askData)
        {
            BidData = bidData;
            AskData = askData;
        }

        public bool Equals(OrderBookRecord other)
        {
            throw new NotImplementedException();
        }

        public BidAskData BidData { get; set; }
        public BidAskData AskData { get; set; }
    }
}
