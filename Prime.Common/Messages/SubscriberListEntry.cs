using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;
using LiteDB;

namespace Prime.Common
{
    public class MessageListEntry<TMessage, TSubscriber> : IDisposable where TMessage : SubscribeMessageBase
    {
        private readonly object _lock = new object();

        private readonly List<TMessage> _entries = new List<TMessage>();

        public TimeSpan ExpirationSpan { get; private set; } = TimeSpan.MaxValue;

        public DateTime UtcLastUpdated { get; private set; }

        public readonly TSubscriber Subscriber;

        public readonly ObjectId SubscriberId;

        public MessageListEntry(TSubscriber subscriber, ObjectId subscriberId)
        {
            Subscriber = subscriber;
            SubscriberId = subscriberId;
            Ping();
        }

        public bool Process(TMessage message)
        {
            ExpirationSpan = message.ExpirationSpan;

            Ping();

            lock (_lock)
            {
                switch (message.SubscriptionType)
                {
                    case SubscriptionType.KeepAlive:
                        return false;
                    case SubscriptionType.Subscribe:
                        return Subscribe(message);
                    case SubscriptionType.Unsubscribe:
                        return Unsubscribe(message);
                    default:
                        return false;
                }
            }
        }

        private bool Subscribe(TMessage message)
        {
            var exists = _entries.FirstOrDefault(x => x.IsEquivalent(message));
            if (exists == null)
                _entries.Add(message);
            else
            {
                _entries.Remove(exists);
                _entries.Add(message);
            }
            return exists != null;
        }

        private bool Unsubscribe(TMessage message)
        {
            var exists = _entries.FirstOrDefault(x => x.IsEquivalent(message));
            if (exists == null)
                return false;
            _entries.Remove(exists);
            return true;
        }

        public void Ping()
        {
            UtcLastUpdated = DateTime.UtcNow;
        }

        public IReadOnlyList<TMessage> GetEntries()
        {
            lock (_lock)
                return _entries.ToList();
        }

        public bool Empty()
        {
            lock (_lock)
                return !_entries.Any();
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _entries.OfType<IDisposable>().ForEach(x => x?.Dispose());
                _entries.Clear();
            }
        }

        public bool IsFresh()
        {
            return ExpirationSpan == TimeSpan.MaxValue || UtcLastUpdated.IsWithinTheLast(ExpirationSpan);
        }
    }
}