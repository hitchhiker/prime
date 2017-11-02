using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
using Prime.Common.Wallet;
using Prime.Core;
using Prime.Utility;
using System.Linq;

namespace Prime.Common.Exchange.Rates
{
    internal class LatestPriceMessenger : RenewingSubscriberList<LatestPriceRequestSubscription, LatestPriceSubscriptions>, IStartupMessenger, IDisposable
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        public readonly LatestPriceAggregator Aggregator;

        internal LatestPriceMessenger()
        {
            Aggregator = new LatestPriceAggregator(this);
        }
        
        protected override LatestPriceSubscriptions OnCreatingSubscriber(ObjectId subscriberId, LatestPriceRequestSubscription message)
        {
            lock (Lock)
                return new LatestPriceSubscriptions();
        }

        protected override void OnAddingToSubscriber(MessageListEntry<LatestPriceRequestSubscription, LatestPriceSubscriptions> entry, LatestPriceRequestSubscription message)
        {
            lock (Lock)
            {
                var subs = entry.Subscriber;
                var nr = new LatestPriceRequest(message.Pair, message.Network);

                if (subs.Requests.Add(nr))
                    nr.Discover();

                Aggregator.SyncProviders();
            }
        }

        protected override void OnRemovingFromSubscriber(MessageListEntry<LatestPriceRequestSubscription, LatestPriceSubscriptions> entry, LatestPriceRequestSubscription message)
        {
            lock (Lock)
            {
                var subs = entry.Subscriber;
                subs.Requests.Remove(new LatestPriceRequest(message.Pair, message.Network));
                Aggregator.SyncProviders();
            }
        }

        protected override void OnRemovingSubscriber(MessageListEntry<LatestPriceRequestSubscription, LatestPriceSubscriptions> entry)
        {
        }

        protected override void OnDisposingSubscription(MessageListEntry<LatestPriceRequestSubscription, LatestPriceSubscriptions> entry)
        {
            //
        }

        public IReadOnlyList<LatestPriceRequest> GetRequests()
        {
            lock (Lock)
                return GetSubscribers().SelectMany(x => x.Requests).ToUniqueList();
        }

        public override void Dispose()
        {
            _messenger.Unregister(this);
        }
    }
}