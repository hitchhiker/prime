using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
using Prime.Common;
using Prime.Common.Wallet;
using Prime.Utility;

namespace Prime.Common
{
    public abstract class SubscriberList<TMessage, TSubscriber> : IDisposable where TMessage  : SubscribeMessageBase
    {
        protected readonly Dictionary<ObjectId, MessageListEntry<TMessage, TSubscriber>> Subscribers = new Dictionary<ObjectId, MessageListEntry<TMessage, TSubscriber>>();

        protected readonly IMessenger M = DefaultMessenger.I.Default;

        protected readonly object Lock = new object();

        protected SubscriberList()
        {
            M.RegisterAsync<TMessage>(this, IncomingSubscription);
        }
        
        protected abstract TSubscriber OnCreatingSubscriber(ObjectId subscriberId, TMessage message);

        protected abstract void OnAddingToSubscriber(MessageListEntry<TMessage, TSubscriber> entry, TMessage message);

        protected abstract void OnRemovingFromSubscriber(MessageListEntry<TMessage, TSubscriber> entry, TMessage message);

        protected abstract void OnRemovingSubscriber(MessageListEntry<TMessage, TSubscriber> entry);

        protected abstract void OnDisposingSubscription(MessageListEntry<TMessage, TSubscriber> entry);

        private void IncomingSubscription(TMessage m)
        {
            if (m == null)
                return;

            lock (Lock)
            {
                switch (m.SubscriptionType)
                {
                    case SubscriptionType.Subscribe:
                        Subscribe(m);
                        break;
                    case SubscriptionType.KeepAlive:
                        KeepAlive(m);
                        break;
                    case SubscriptionType.Unsubscribe:
                        Unsubscribe(m);
                        break;
                    case SubscriptionType.UnsubscribeAll:
                        UnsubscribeAll(m);
                        break;
                }
            }
        }

        private void Subscribe(TMessage message)
        {
            var e = Subscribers.Get(message.SubscriberId);
            if (e == null)
            {
                var subscriber = OnCreatingSubscriber(message.SubscriberId, message);
                Subscribers.Add(message.SubscriberId, e = new MessageListEntry<TMessage, TSubscriber>(subscriber, message.SubscriberId));
            }

            //only add if not an update

            if (!e.Process(message))
                OnAddingToSubscriber(e, message);
        }

        private void KeepAlive(TMessage message)
        {
            Subscribers.Get(message.SubscriberId)?.Ping();
        }

        private void Unsubscribe(TMessage message)
        {
            var e = Subscribers.Get(message.SubscriberId);
            if (e == null)
                return;

            e.Process(message);

            OnRemovingFromSubscriber(e, message);

            TryRemoveSubscriber(e);
        }

        private void UnsubscribeAll(TMessage message)
        {
            UnsubscribeAll(message.SubscriberId);
        }

        protected void UnsubscribeAll(ObjectId subscriberId)
        {
            lock (Lock)
            {
                var e = Subscribers.Get(subscriberId);

                if (e == null)
                    return;

                foreach (var i in e.GetEntries())
                {
                    e.Unsubscribe(i);
                    OnRemovingFromSubscriber(e, i);
                }

                TryRemoveSubscriber(e);
            }
        }

        private void TryRemoveSubscriber(MessageListEntry<TMessage, TSubscriber> e)
        {
            if (!e.Empty())
                return;

            OnRemovingSubscriber(e);

            Subscribers.Remove(e.SubscriberId);

            OnDisposingSubscription(e);

            e.Dispose();
        }

        public IReadOnlyList<MessageListEntry<TMessage, TSubscriber>> GetEntries()
        {
            lock (Lock)
                return Subscribers.Values.ToList();
        }

        public IReadOnlyList<TSubscriber> GetSubscribers()
        {
            lock (Lock)
                return Subscribers.Values.Select(x=>x.Subscriber).ToList();
        }

        public IReadOnlyList<TMessage> GetMessages()
        {
            lock (Lock)
                return Subscribers.Values.SelectMany(x => x.GetEntries()).ToList();
        }

        public virtual void Dispose()
        {
            lock (Lock)
            {
                foreach (var kv in Subscribers)
                    UnsubscribeAll(kv.Key);
                
                Subscribers.Clear();
                M.Unregister(this);
            }
        }
    }
}