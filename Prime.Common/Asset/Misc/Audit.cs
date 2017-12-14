using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class Audit
    {
        private readonly object _lock = new object();

        private Audit() { }

        public Audit(Money? startingBalance = null)
        {
            if (startingBalance != null)
                Add(startingBalance.Value);
            else
                CurrentBalance = Money.Zero;
        }

        [Bson]
        public DateTime UtcCreated { get; private set; } = DateTime.UtcNow;

        [Bson]
        private List<AuditEntry> Audits { get; set; } = new List<AuditEntry>();

        [Bson]
        public Money CurrentBalance { get; private set; }

        public AuditEntry GetCurrent(Asset asset)
        {
            lock (_lock)
                return Audits.Where(x => x.Value.Asset.Id == asset.Id).OrderByDescending(x=>x.UtcCreated).FirstOrDefault();
        }

        public Money? GetCurrentBalance(Asset asset)
        {
            return GetCurrent(asset)?.Value;
        }

        public IReadOnlyList<AuditEntry> Get()
        {
            lock (_lock)
                return Audits.ToList();
        }

        public IReadOnlyList<AuditEntry> Get(Asset asset)
        {
            lock (_lock)
                return Audits.Where(x => x.Value.Asset.Id == asset.Id).ToList();
        }
        
        public Money StartingBalance
        {
            get
            {
                lock (_lock)
                    return Audits.Any() ? Audits.First().Value : Money.Zero;
            }
        }

        public void Add(Money balance, MarketPrice price = null)
        {
            lock (_lock)
            {
                CurrentBalance = balance;
                Audits.Add(new AuditEntry(DateTime.UtcNow, balance, price));
            }
        }

        public AuditEntry Previous()
        {
            lock (_lock)
            {
                var c = CurrentBalance;

                if (c == null)
                    return null;

                var aud = GetCurrent(c.Asset);

                return aud == null ? 
                    null : 
                    Audits.Where(x => x.Value.Asset.Id == CurrentBalance.Asset.Id && x != aud).OrderByDescending(x => x.UtcCreated).FirstOrDefault();
            }
        }

        public string ToTextFile()
        {
            var sb = new StringBuilder();
            lock (_lock)
            {
                foreach (var entry in Get())
                    sb.Append(entry + Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}