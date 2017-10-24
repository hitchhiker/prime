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
    internal class LatestPriceMessenger : RenewingSubscriberList<LatestPriceRequestMessage, LatestPriceSubscriptions>, IStartupMessenger, IDisposable
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        public readonly LatestPriceAggregator Aggregator;

        internal LatestPriceMessenger()
        {
            Aggregator = new LatestPriceAggregator(this);
            _messenger.Register<InternalLatestPriceRequestVerifiedMessage>(this, Aggregator.LatestPriceRequestVerifiedMessage);
            _messenger.Register<LatestPriceResultMessage>(this, Aggregator.LatestPriceResultMessage);
        }
        
        protected override LatestPriceSubscriptions OnCreatingSubscriber(ObjectId subscriberId, LatestPriceRequestMessage message)
        {
            return new LatestPriceSubscriptions();
        }

        protected override void OnAddingToSubscriber(MessageListEntry<LatestPriceRequestMessage, LatestPriceSubscriptions> entry, LatestPriceRequestMessage message)
        {
            var subs = entry.Subscriber;
            var nr = new LatestPriceRequest(message.Pair, message.Network);
            if (subs.Requests.Add(nr))
                nr.Discover();
            Aggregator.SyncProviders();
        }

        protected override void OnRemovingFromSubscriber(MessageListEntry<LatestPriceRequestMessage, LatestPriceSubscriptions> entry, LatestPriceRequestMessage message)
        {
            var subs = entry.Subscriber;
            subs.Requests.Remove(new LatestPriceRequest(message.Pair, message.Network));
            Aggregator.SyncProviders();
        }

        protected override void OnRemovingSubscriber(MessageListEntry<LatestPriceRequestMessage, LatestPriceSubscriptions> entry)
        {
        }

        protected override void OnDisposingSubscription(MessageListEntry<LatestPriceRequestMessage, LatestPriceSubscriptions> entry)
        {
            //
        }

        public IReadOnlyList<LatestPriceRequest> GetRequests()
        {
            return GetSubscribers().SelectMany(x => x.Requests).ToUniqueList();
        }

        public override void Dispose()
        {
            _messenger.Unregister(this);
        }
    }
}