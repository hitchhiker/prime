using System;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Utility
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
                action(element);
        }

        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int pageIndex, int pageSize)
        {
            return pageIndex == 0
                ? source.Take(pageSize)
                : source.Skip(pageIndex * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Obtains every nth item from the sequence
        /// </summary>
        public static IEnumerable<T> EveryNth<T>(this IEnumerable<T> source, int nth)
        {
            return source.Where((x, i) => i % nth == 0);
        }

        /// <summary>
        /// Splits the sequence into blocks of 'size' and returns items at the supplied index position within each block.
        /// </summary>
        public static IEnumerable<T> EveryNthIndex<T>(this IEnumerable<T> source, int size, int index)
        {
            return source.Where((x, i) => i % size == index);
        }

        public static TResult MaxOrDefault<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector, TResult defaultValue)
        {
            if (source==null || !source.Any())
                return defaultValue;
            try
            {
                return source.Max(selector);
            }
            catch //try catch to avoid multiple enumeration :(
            {
                return defaultValue;
            }
        }

        public static TResult MinOrDefault<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector, TResult defaultValue)
        {
            if (source == null || !source.Any())
                return defaultValue;

            try
            {
                return source.Min(selector);
            }
            catch //try catch to avoid multiple enumeration :(
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns a new List of Type T from objects within the source list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>

        public static List<T> OfList<T>(this IEnumerable<object> source)
        {
            return source.OfType<T>().ToList();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var i = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (i.Add(keySelector(element)))
                    yield return element;
            }
        }

        public static IEnumerable<TSource> FilterType<TSource, TFilter>(this IEnumerable<TSource> source)
        {
            return source.OfType<TFilter>().OfType<TSource>();
        }
    }
}
