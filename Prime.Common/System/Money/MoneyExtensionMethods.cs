using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Prime.Common
{
    public static class MoneyExtensionMethods
    {
        public static Money? Sum(this IEnumerable<Money?> items, bool allowNull, Asset baseAsset = null)
        {
            var r = Sum(items, baseAsset);
            if (allowNull && r == Money.Zero)
                return null;
            return r;
        }

        public static Money Sum(this IEnumerable<Money?> items, Asset baseAsset = null)
        {
            return Sum(items.Where(x => x != null).Select(x => x.Value), baseAsset);
        }

        public static Money Sum(this IEnumerable<Money> items, Asset baseAsset = null)
        {
            if (items == null || !items.Any())
                return new Money(0, baseAsset ?? Asset.None);

            var ms = items.Where(x=>x!=null).ToList();

            if (!ms.Any())
                return new Money(0, baseAsset ?? Asset.None);

            var c = baseAsset ?? ms.FirstOrDefault(x=>!Equals(x.Asset, Asset.None)).Asset;

            if (ms.Any(x=>!Equals(x, Money.Zero) && !x.Asset.Equals(c)))
                throw new ArgumentException("When summing money items, each item must either be in given currency or Zero: " + c.ShortCode);

            return new Money(ms.Sum(x=>x), c);
        }

        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         Decimal distribution)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(distribution);
        }

        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         Decimal distribution1,
                                         Decimal distribution2)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(distribution1,
                                                                                             distribution2);
        }

        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         Decimal distribution1,
                                         Decimal distribution2,
                                         Decimal distribution3)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(distribution1,
                                                                                             distribution2,
                                                                                             distribution3);
        }

        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         params Decimal[] distributions)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(distributions);
        }

        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         Int32 count)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(count);
        }

        public static decimal PercentageDifference(this Money money1, Money money2)
        {
            if (money1 == 0 && money2 == 0)
                return 0;

            if (!Equals(money1.Asset, money2.Asset))
                return 100;

            if (money1 == 0)
                return Math.Abs(((decimal)(money1 - money2) / Math.Abs(money2)) * 100);

            return Math.Abs((((decimal)(money2 - money1) / Math.Abs(money1))) * 100);
        }

        public static bool IsWithinPercentage(this Money money1, Money money2, decimal percentageTolerance)
        {
            return PercentageDifference(money1, money2) <= percentageTolerance;
        }
    }
}