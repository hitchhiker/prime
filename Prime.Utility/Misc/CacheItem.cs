using System;
using Prime.Common;

namespace Prime.Common
{
    public class CacheItem<T>
    {
        public readonly DateTime UtcCreated;

        public readonly TimeSpan ExpirationSpan;

        public readonly T Value;

        public CacheItem(TimeSpan expiresIn, T value)
        {
            ExpirationSpan = expiresIn;
            Value = value;
            UtcCreated = DateTime.UtcNow;
        }

        public CacheItem(TimeSpan expiresIn, DateTime utcCreated, T value)
        {
            ExpirationSpan = expiresIn;
            UtcCreated = utcCreated;
            Value = value;
        }

        public bool IsFresh => UtcCreated.IsWithinTheLast(ExpirationSpan);
    }
}