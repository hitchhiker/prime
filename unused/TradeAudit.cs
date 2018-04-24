using System;

namespace Prime.Common
{
    public class TradeAudit
    {
        [Bson]
        public DateTime UtcCreated { get; set; }

        [Bson]
        public MarketPrice Price { get; set; }
    }
}