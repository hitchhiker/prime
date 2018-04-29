using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
using Prime.Core;
using System.Linq;

namespace Prime.Finance.Exchange.Rates
{
    //internal class ExchangeRateMessenger : RenewingSubscriberList<ExchangeRateSubscribe, ExchangeRateSubscriptions>, IStartupMessenger, IDisposable
    //{
    //    private readonly IMessenger _messenger = DefaultMessenger.I.Default;

    //    internal ExchangeRateMessenger()
    //    {
    //        _messenger.Register<LatestPriceResultMessage>(this, LatestPriceResultMessage);
    //    }

    //    private void LatestPriceResultMessage(LatestPriceResultMessage m)
    //    {
    //    }

    //    protected override ExchangeRateSubscriptions OnCreatingSubscriber(ObjectId subscriberId, ExchangeRateSubscribe message)
    //    {
    //        return new ExchangeRateSubscriptions();
    //    }

    //    protected override void OnAddingToSubscriber(MessageListEntry<ExchangeRateSubscribe, ExchangeRateSubscriptions> entry, ExchangeRateSubscribe message)
    //    {
    //        var subs = entry.Subscriber;
    //        var nr = new LatestPriceRequest(message.Pair, message.Network);

    //        if (subs.Requests.Add(nr))
    //            nr.Discover();

    //    }

    //    protected override void OnRemovingFromSubscriber(MessageListEntry<ExchangeRateSubscribe, ExchangeRateSubscriptions> entry, ExchangeRateSubscribe message)
    //    {
    //        var subs = entry.Subscriber;
    //        subs.Requests.Remove(new LatestPriceRequest(message.Pair, message.Network));
    //        Aggregator.SyncProviders();
    //    }

    //    protected override void OnRemovingSubscriber(MessageListEntry<ExchangeRateSubscribe, ExchangeRateSubscriptions> entry)
    //    {
    //    }

    //    protected override void OnDisposingSubscription(MessageListEntry<ExchangeRateSubscribe, ExchangeRateSubscriptions> entry)
    //    {
    //        //
    //    }

    //    public override void Dispose()
    //    {
    //        _messenger.Unregister(this);
    //    }
    //}
}