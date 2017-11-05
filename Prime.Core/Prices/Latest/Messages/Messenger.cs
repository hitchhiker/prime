using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Utility;

namespace Prime.Core.Prices.Latest
{
    internal sealed class Messenger : RenewingSubscriberList<LatestPriceRequestSubscription, SubscriptionRequests>, IStartupMessenger, IDisposable
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        public readonly Aggregator Aggregator;

        internal Messenger()
        {
            Aggregator = new Aggregator(this);
        }
        
        protected override SubscriptionRequests OnCreatingSubscriber(ObjectId subscriberId, LatestPriceRequestSubscription message)
        {
            lock (Lock)
                return new SubscriptionRequests();
        }

        protected override void OnAddingToSubscriber(MessageListEntry<LatestPriceRequestSubscription, SubscriptionRequests> entry, LatestPriceRequestSubscription message)
        {
            lock (Lock)
            {
                var subs = entry.Subscriber;
                var nr = new Request(message.Pair, message.Network);

                var request = subs.Requests.FirstOrDefault(x => x.Equals(nr));
                if (request != null)
                    return;

                subs.Requests.Add(request = nr);
                request.Processor = new DiscoveryRequestProcessor(request, () =>
                {
                    request.Messenger = new RequestMessenger(request);
                    Aggregator.SyncProviders();
                });
            }
        }

        protected override void OnRemovingFromSubscriber(MessageListEntry<LatestPriceRequestSubscription, SubscriptionRequests> entry, LatestPriceRequestSubscription message)
        {
            lock (Lock)
            {
                var subs = entry.Subscriber;

                var nr = new Request(message.Pair, message.Network);
                var request = subs.Requests.FirstOrDefault(x => x.Equals(nr));
                if (request == null)
                    return;

                request.Dispose();
                subs.Requests.Remove(request);
                Aggregator.SyncProviders();
            }
        }

        protected override void OnRemovingSubscriber(MessageListEntry<LatestPriceRequestSubscription, SubscriptionRequests> entry)
        {
        }

        protected override void OnDisposingSubscription(MessageListEntry<LatestPriceRequestSubscription, SubscriptionRequests> entry)
        {
            //
        }

        public IReadOnlyList<Request> GetRequests()
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