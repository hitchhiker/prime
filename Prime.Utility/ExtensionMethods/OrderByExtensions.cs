using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

//mod from: http://aonnull.blogspot.com/2010/08/dynamic-sql-like-linq-orderby-extension.html
using LiteDB;

namespace Prime.Utility
{
    public static class OrderByExtensions
    {
        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortingDirection direction)
        {
            return direction == SortingDirection.Ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
        }

        public static IQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, SortingDirection direction)
        {
            return direction == SortingDirection.Ascending ? source.OrderBy(keySelector).AsQueryable() : source.OrderByDescending(keySelector).AsQueryable();
        }
        
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string property)
        {
            return enumerable.AsQueryable().OrderBy(property).AsEnumerable();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection, string property)
        {
            return ParseOrderBy(property, SortingDirection.Ascending).Aggregate(collection, ApplyOrderBy);
        }

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> enumerable, string property)
        {
            return enumerable.AsQueryable().OrderByDescending(property).AsEnumerable();
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> collection, string property)
        {
            return ParseOrderBy(property, SortingDirection.Descending).Aggregate(collection, ApplyOrderBy);
        }

        private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> collection, OrderByInfo orderByInfo)
        {
            var props = orderByInfo.PropertyName.Split('.');
            var type = typeof(T);

            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            var type1 = type;
            foreach (var pi in props.Select(type1.GetProperty))
            {
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var methodName = !orderByInfo.Initial && collection is IOrderedQueryable<T> ? 
                (orderByInfo.Direction == SortingDirection.Ascending ? "ThenBy" : "ThenByDescending") :                                                                                                 
                (orderByInfo.Direction == SortingDirection.Ascending ? "OrderBy" : "OrderByDescending");

            //TODO: apply caching to the generic methodsinfos?
            return (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { collection, lambda });

        }

        private static IEnumerable<OrderByInfo> ParseOrderBy(string orderBy, SortingDirection sortorder)
        {
            if (String.IsNullOrEmpty(orderBy))
                yield break;

            var items = orderBy.Split(',');
            var initial = true;
            foreach (var item in items)
            {
                yield return new OrderByInfo { PropertyName = item, Direction = sortorder, Initial = initial };
                initial = false;
            }
        }

        private class OrderByInfo
        {
            public string PropertyName { get; set; }
            public SortingDirection Direction { get; set; }
            public bool Initial { get; set; }
        }
    }
}
