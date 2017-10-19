using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Utility
{
    public class CacheDictionary<T, T2>
    {
        public readonly TimeSpan ExpirationSpan;

        public CacheDictionary(TimeSpan expirationSpan)
        {
            ExpirationSpan = expirationSpan;
        }

        private readonly object _lock = new object();

        private readonly Dictionary<T, CacheItem<T2>> _cache = new Dictionary<T, CacheItem<T2>>();

        public T2 Try(T key, Func<T, T2> create)
        {
            lock (_lock)
            {
                var ci = _cache.Get(key);

                if (ci != null)
                    if (!ci.IsFresh)
                        _cache.Remove(key);
                    else
                        return ci.Value;

                var o = create(key);

                ci = new CacheItem<T2>(ExpirationSpan, o);
                _cache.Add(key, ci);
                return o;
            }
        }
    }
}