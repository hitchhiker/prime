using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common.Exchange.Rates
{
    public class SubscriberList<T>
    {
        private readonly Action<T> _onExisting;
        private readonly Action<T> _onAdded;
        private readonly Action<T> _onRemoved;

        public SubscriberList(Action<T> onAdded, Action<T> onExisting, Action<T> onRemoved)
        {
            _onExisting = onExisting;
            _onAdded = onAdded;
            _onRemoved = onRemoved;
        }

        private readonly object _commonLock = new object();
        private readonly Dictionary<WeakReference, List<T>> _subscribers = new Dictionary<WeakReference, List<T>>();

        public void GarbageCollect()
        {
            lock (_commonLock)
            {
                var expired = _subscribers.Keys.Where(x => !x.IsAlive).ToList();
                foreach (var k in expired)
                {
                    var v = _subscribers.Get(k) ?? new List<T>();
                    _subscribers.Remove(k);
                    foreach (var i in v)
                        _onRemoved.Invoke(i);
                }
            }
        }

        public T Subscribe(object subscriber, T subscription)
        {
            lock (_commonLock)
            {
                List<T> l = null;
                var kv = _subscribers.FirstOrDefault(x => x.Key.Target?.Equals(subscriber) == true);
                if (kv.Key?.IsAlive == true)
                    l = kv.Value;
                else
                {
                    if (kv.Key != null)
                        _subscribers.Remove(kv.Key);
                    _subscribers.Add(new WeakReference(subscriber), l = new List<T>());
                }

                var existingReq = l.FirstOrDefault(x => x.Equals(subscription));
                if (existingReq != null)
                {
                    _onExisting.Invoke(existingReq);
                    return existingReq;
                }

                l.Add(subscription);
                _onAdded.Invoke(subscription);
                return subscription;
            }
        }

        public void Unsubscribe(object subscriber, T subscription)
        {
            lock (_commonLock)
            {
                if (subscription == null)
                    return;

                var kv = _subscribers.FirstOrDefault(x => x.Key.Target?.Equals(subscriber)==true);
                if (kv.Key == null)
                    return;

                kv.Value.Remove(subscription);

                if (!kv.Value.Any())
                    _subscribers.Remove(kv.Key);

                _onRemoved.Invoke(subscription);
            }
        }

        public IReadOnlyList<T> GetSubscriptions()
        {
            lock (_commonLock)
            {
                return _subscribers.SelectMany(x => x.Value).ToList();
            }
        }
    }
}