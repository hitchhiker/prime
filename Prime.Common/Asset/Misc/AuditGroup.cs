using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;

namespace Prime.Common
{
    public class AuditGroup
    {
        public AuditGroup(Money startingBalance, MarketPrice price = null)
        {
            Asset = startingBalance.Asset;
            Add(startingBalance, price);
        }

        private readonly object _lock = new object();

        public Money StartingBalance
        {
            get
            {
                lock (_lock)
                    return Audit.First().Balance;
            }
        }

        public Money FinalBalance
        {
            get
            {
                lock (_lock)
                    return Audit.Last().Balance;
            }
        }

        [Bson]
        public Asset Asset { get; private set; }

        [Bson]
        private List<AuditEntry> Audit { get; set; } = new List<AuditEntry>();

        [Bson]
        public MarketPrice Price { get; set; }

        public IReadOnlyList<AuditEntry> Audits
        {
            get
            {
                lock (_lock)
                    return Audit.ToList();
            }
        }

        public void Add(Money balance, MarketPrice price = null)
        {
            if (balance.Asset.Id != Asset.Id)
                throw new Exception($"You cant add this balance to {nameof(AuditGroup)} as it has the wrong asset class: {balance.Asset.ShortCode} needs {Asset.ShortCode}");

            lock (_lock)
                Audit.Add(new AuditEntry(DateTime.UtcNow, balance, price));
        }
    }
}