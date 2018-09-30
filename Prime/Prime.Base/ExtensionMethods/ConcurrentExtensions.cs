using System.Collections.Concurrent;

namespace Prime.Core
{
    public static class ConcurrentExtensions
    {
        public static void Clear<T>(this ConcurrentBag<T> bag)
        {
            while (!bag.IsEmpty)
                bag.TryTake(out _);
        }
    }
}