using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class Audit
    {
        private readonly object _lock = new object();

        private Audit() { }

        public Audit(Money startingBalance)
        {
            SetCurrent(startingBalance);
        }

        [Bson]
        private Dictionary<Asset, AuditGroup> Audits { get; set; } = new Dictionary<Asset, AuditGroup>();

        [Bson]
        private List<TradeAudit> Trades { get; set; } = new List<TradeAudit>();

        [Bson]
        public Money CurrentBalance { get; private set; }

        public AuditGroup Get(Asset asset)
        {
            lock (_lock)
                return Audits.Get(asset);
        }

        public MarketPrice GetLatest(AssetPair pair)
        {
            var ta = Trades.OrderByDescending(x=>x.UtcCreated).FirstOrDefault(x => x.Price.Pair.EqualsOrReversed(pair));
            return ta?.Price.AsQuote(pair.Asset2);
        }

        public Money StartingBalance
        {
            get
            {
                lock (_lock)
                    return Audits.First().Value.StartingBalance;
            }
        }

        public AuditGroup SetCurrent(Money balance, MarketPrice price = null)
        {
            lock (_lock)
            {
                if (!Audits.ContainsKey(balance.Asset))
                    Audits.Add(balance.Asset, new AuditGroup(balance));
                else
                    Audits[balance.Asset].Add(balance);

                CurrentBalance = balance;

                if (price != null)
                    Trades.Add(new TradeAudit() {UtcCreated = DateTime.UtcNow, Price = price});

                return Audits[balance.Asset];
            }
        }

        public List<AuditGroup> All()
        {
            lock (_lock)
                return Audits.Values.ToList();
        }
    }
}