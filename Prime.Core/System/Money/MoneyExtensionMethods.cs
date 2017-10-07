using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Prime.Core
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
    }
}