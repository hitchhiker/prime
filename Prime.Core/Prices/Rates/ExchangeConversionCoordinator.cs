using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Common.Exchange.Rates
{
    public class ExchangeConversionCoordinator
    {
        private readonly LatestPriceAggregator _c;
        private readonly WeakSubscriberList<FxRequestMessage> _requests;

        public static ExchangeConversionCoordinator I => Lazy.Value;
        private static readonly Lazy<ExchangeConversionCoordinator> Lazy = new Lazy<ExchangeConversionCoordinator>(() => new ExchangeConversionCoordinator());

        private ExchangeConversionCoordinator()
        {
            _requests = new WeakSubscriberList<FxRequestMessage>(OnAddedSubscription, OnExistingSubscription, OnRemovedSubscription);
        }

        public void Register(object subscriber, FxRequestMessage requestMessage)
        {
            _requests.Subscribe(subscriber, requestMessage);
            requestMessage.Register();
        }

        public void Unregister(object subscriber, FxRequestMessage requestMessage)
        {
            _requests.Unsubscribe(subscriber, requestMessage); 
        }

        private void OnRemovedSubscription(FxRequestMessage requestMessage)
        {
            requestMessage.Unregister();
        }

        private void OnAddedSubscription(FxRequestMessage requestMessage)
        {

        }

        private void OnExistingSubscription(FxRequestMessage requestMessage)
        {

        }
    }
}