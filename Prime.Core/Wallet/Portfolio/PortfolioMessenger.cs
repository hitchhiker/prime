using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
using Prime.Common;
using Prime.Common.Wallet;
using Prime.Core;
using Prime.Utility;

namespace Prime.Core.Wallet
{
    internal class PortfolioMessenger : RenewingSubscriberList<PortfolioSubscribeMessage, PortfolioAggregator>, IStartupMessenger
    {
        protected override PortfolioAggregator OnCreatingSubscriber(ObjectId subscriberId, PortfolioSubscribeMessage message)
        {
            var aggregator = new PortfolioAggregator(UserContext.Get(message.UserContextId));
            aggregator.Start();
            return aggregator;
        }

        protected override void OnAddingToSubscriber(MessageListEntry<PortfolioSubscribeMessage, PortfolioAggregator> entry, PortfolioSubscribeMessage message)
        {
            var aggregator = entry.Subscriber;
            aggregator.SendChangedMessage();
        }

        protected override void OnRemovingFromSubscriber(MessageListEntry<PortfolioSubscribeMessage, PortfolioAggregator> entry, PortfolioSubscribeMessage message)
        {
            //
        }

        protected override void OnRemovingSubscriber(MessageListEntry<PortfolioSubscribeMessage, PortfolioAggregator> entry)
        {
            //
        }

        protected override void OnDisposingSubscription(MessageListEntry<PortfolioSubscribeMessage, PortfolioAggregator> entry)
        {
            var aggregator = entry.Subscriber;
            aggregator.Stop();
        }
    }
}