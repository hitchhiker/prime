using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Core;

namespace Prime.Core
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

        private static readonly Random Random = new Random();

        /// <summary>
        /// Gets a random element from a callection of elements that implments the IEnumerable interface
        /// </summary>
        /// <typeparam name="T">The type of each element in the collection</typeparam>
        /// <param name="list">A collection of elements of type T</param>
        /// <returns>Returns a random element from a collection</returns>        
        public static T GetRandomElement<T>(this IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            // Get the number of elements in the collection
            int count = list.Count();

            // If there are no elements in the collection, return the default value of T
            if (count == 0)
                return default(T);

            // Get a random index
            int index = Random.Next(list.Count());

            // When the collection has 100 elements or less, get the random element
            // by traversing the collection one element at a time.
            if (count <= 100)
            {
                using (IEnumerator<T> enumerator = list.GetEnumerator())
                {
                    // Move down the collection one element at a time.
                    // When index is -1 we are at the random element location
                    while (index >= 0 && enumerator.MoveNext())
                        index--;

                    // Return the current element
                    return enumerator.Current;
                }
            }

            // Get an element using LINQ which casts the collection
            // to an IList and indexes into it.
            return list.ElementAt(index);
        }
    }
}