using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Core;
using Prime.Utility;

namespace Prime.Core.Wallet
{
    public class PortfolioGroupedItem : IEquatable<PortfolioGroupedItem>
    {
        public PortfolioGroupedItem(Asset asset)
        {
            Asset = asset;
        }

        public static PortfolioGroupedItem Create(Asset asset, List<PortfolioLineItem> items)
        {
            var cva = items.FirstOrDefault()?.Converted.Asset ?? Asset.None;

            var g = new PortfolioGroupedItem(asset)
            {
                AvailableBalance = new Money(items.Sum(x => x.AvailableBalance), asset),
                PendingBalance = new Money(items.Sum(x => x.PendingBalance), asset),
                ReservedBalance = new Money(items.Sum(x => x.ReservedBalance), asset),
                Total = new Money(items.Sum(x => x.Total), asset),
                Converted = new Money(items.Sum(x => x.Converted), cva),
                ConversionFailed = items.Any(x => x.ConversionFailed),
                IsTotalLine = asset == null
            };

            return g;
        }

        public Asset Asset { get; private set; }

        public string Id => Asset.ShortCode;

        public bool IsTotalLine { get; set; }

        public Money AvailableBalance { get; set; }

        public Money PendingBalance { get; set; }

        public Money ReservedBalance { get; set; }

        public Money Total { get; set; }

        public Money Converted { get; set; }

        public bool ConversionFailed { get; set; }

        public double ChangePercentage { get; set; }

        public bool Equals(PortfolioGroupedItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Asset, other.Asset);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PortfolioGroupedItem) obj);
        }

        public override int GetHashCode()
        {
            return (Asset != null ? Asset.GetHashCode() : 0);
        }

    }
}