using System;
using System.Text;

namespace Prime.Common
{
    public class TradeOrder
    {
        public TradeOrder(string remoteOrderId, Network network, AssetPair pair, TradeOrderType orderType)
        {
            Network = network;
            RemoteOrderId = remoteOrderId.Trim();
            Pair = pair;
            OrderType = orderType;
        }

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public string RemoteOrderId {get; private set; }

        [Bson]
        public AssetPair Pair {get; private set; }

        [Bson]
        public TradeOrderType OrderType {get; private set; }

        [Bson]
        public decimal Quantity {get; set; }

        [Bson]
        public decimal QuantityRemaining {get; set; }

        [Bson]
        public Money Limit {get; set; }

        [Bson]
        public Money CommissionPaid {get; set; }

        [Bson]
        public Money Price {get; set; }

        [Bson]
        public Money? PricePerUnit {get; set; }

        [Bson]
        public DateTime? Opened {get; set; }

        [Bson]
        public DateTime? Closed {get; set; }

        [Bson]
        public bool CancelInitiated {get; set; }

        [Bson]
        public bool ImmediateOrCancel {get; set; }

        [Bson]
        public bool IsConditional {get; set; }

        [Bson]
        public string Condition {get; set; }

        [Bson]
        public string ConditionTarget {get; set;}

        public override string ToString()
        {
            return $"{Network.Name} {Pair} {Quantity} {Price}";
        }
    }
}
