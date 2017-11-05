using System;
using System.Collections.Concurrent;
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

        private readonly Dictionary<T, CacheItem<T2>> _cache = new Dictionary<T, CacheItem<T2>>();
        private readonly ConcurrentDictionary<T, object> _locks = new ConcurrentDictionary<T, object>();

        public T2 Try(T key, Func<T, T2> create)
        {
            var locki = _locks.GetOrAdd(key, k => new object());

            lock (locki)
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