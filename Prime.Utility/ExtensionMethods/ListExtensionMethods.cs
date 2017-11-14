using System;
using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Utility
{
    public static class ListExtensionMethods
    {
        public static int IndexOf<T>(this IReadOnlyList<T> source, T item)
        {
            switch (source)
            {
                case IList<T> l:
                    return l.IndexOf(item);
                case IEnumerable<T> en:
                    return en.IndexOf(item);
            }

            throw new Exception($"Can't perform {nameof(IndexOf)} as the source list is incompatible.");
        }
    }
}