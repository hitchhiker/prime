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
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(
                BidData.Equals(other.BidData) &&
                AskData.Equals(other.AskData) &&
                BidData.Time.Equals(other.BidData.Time)
            );
        }

        public BidAskData BidData { get; set; }
        public BidAskData AskData { get; set; }

        public override string ToString()
        {
            return $"Bid: {BidData.Price.Display} Ask: {AskData.Price.Display}";
        }
    }
}
