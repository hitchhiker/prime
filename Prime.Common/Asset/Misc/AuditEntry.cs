using System;
using LiteDB;

namespace Prime.Common
{
    public class AuditEntry
    {
        private AuditEntry() { }

        public AuditEntry(DateTime utcCreated, Money value, MarketPrice tradePrice = null)
        {
            UtcCreated = utcCreated;
            Value = value;
            TradePrice = tradePrice;
        }

        [Bson]
        public DateTime UtcCreated { get; private set; }

        [Bson]
        public Money Value { get; private set; }

        [Bson]
        public MarketPrice TradePrice { get; private set; }
    }
}