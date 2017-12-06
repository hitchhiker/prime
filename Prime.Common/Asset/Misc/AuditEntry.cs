using System;

namespace Prime.Common
{
    public class AuditEntry
    {
        private AuditEntry() { }

        public AuditEntry(DateTime utcCreated, Money balance, MarketPrice tradePrice = null)
        {
            UtcCreated = utcCreated;
            Balance = balance;
            TradePrice = tradePrice;
        }

        public DateTime UtcCreated { get; private set; }

        public Money Balance { get; private set; }

        public MarketPrice TradePrice { get; private set; }
    }
}