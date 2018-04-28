using System;
using System.Collections.Generic;

namespace Prime.Common
{
    public interface IUniqueList<T> : IList<T> where T : IEquatable<T>
    {
        bool Add(T item, bool readd = false);
        bool AddRange(IEnumerable<T> items);
        IEnumerable<T> Distinct();
    }
}