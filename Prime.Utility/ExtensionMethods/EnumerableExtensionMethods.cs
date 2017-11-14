using System;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Utility
{
    public static class EnumerableExtensionMethods
    {
        /// <summary>
        /// If possibleList is IEnumerable then convert to IReadOnlyList, otherwise just case to IReadOnlyList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="possibleList"></param>
        /// <returns></returns>
        public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> possibleList)
        {
            if (possibleList == null)
                return default;

            return possibleList as IReadOnlyList<T> ?? possibleList.ToList();
        }

        /// <summary>
        /// If possibleList is IEnumerable then convert to List, otherwise just case to List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="possibleList"></param>
        /// <returns></returns>
        public static List<T> AsList<T>(this IEnumerable<T> possibleList)
        {
            if (possibleList == null)
                return default;

            return possibleList as List<T> ?? possibleList.ToList();
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            return IndexOf(source, x => x.Equals(value));
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> equalityPredicate)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (equalityPredicate(item))
                    return index;
                index++;
            }
            return -1;
        }
    }
}