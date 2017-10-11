using System;
using Prime.Core;
using Prime.Utility;

namespace Prime.Core.Wallet
{
    public class PortfolioLineItem : IEquatable<PortfolioLineItem>
    {
        public string Id => Network.Id + ":" + Asset.ShortCode;

        public virtual bool IsTotalLine => false;

        public Network Network { get; set; }

        public Asset Asset { get; set; }

        public Money AvailableBalance { get; set; }

        public Money PendingBalance { get; set; }

        public Money ReservedBalance { get; set; }

        public Money Total { get; set; }

        public virtual Money? Converted { get; set; }

        public bool ConversionFailed { get; set; }

        public double ChangePercentage { get; set; }

        public void Update(PortfolioLineItem p)
        {
            AvailableBalance = p.AvailableBalance;
            PendingBalance = p.PendingBalance;
            ReservedBalance = p.ReservedBalance;
            Total = p.Total;
            ChangePercentage = p.ChangePercentage;
            Converted = null;
        }

        public bool Equals(PortfolioLineItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Network, other.Network) && Equals(Asset, other.Asset);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PortfolioLineItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Network != null ? Network.GetHashCode() : 0) * 397) ^ (Asset != null ? Asset.GetHashCode() : 0);
            }
        }
    }
}