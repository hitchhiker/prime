using System;
using System.Collections;
using System.Collections.Generic;

namespace Prime.Utility
{
    public static class UniqueListExtensions
    {
        public static UniqueList<T> ToUniqueList<T>(this IEnumerable<T> source) where T : IEquatable<T>
        {
            return new UniqueList<T>(source);
        } 
    }
}