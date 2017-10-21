using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Common.Exchange.Rates
{
    public class ExchangeConversionCoordinator
    {
        private readonly LatestPriceCoordinator _c;
        private readonly SubscriberList<ExchangeConversionRequest> _requests;

        public static ExchangeConversionCoordinator I => Lazy.Value;
        private static readonly Lazy<ExchangeConversionCoordinator> Lazy = new Lazy<ExchangeConversionCoordinator>(() => new ExchangeConversionCoordinator());

        private ExchangeConversionCoordinator()
        {
            _c = LatestPriceCoordinator.I;
            _requests = new SubscriberList<ExchangeConversionRequest>(OnAddedSubscription, OnExistingSubscription, OnRemovedSubscription);
        }

        public void Register(object subscriber, ExchangeConversionRequest request)
        {
            _requests.Subscribe(subscriber, request);
            request.Register();
        }

        public void Unregister(object subscriber, ExchangeConversionRequest request)
        {
            _requests.Unsubscribe(subscriber, request); 
        }

        private void OnRemovedSubscription(ExchangeConversionRequest request)
        {
            request.Unregister();
        }

        private void OnAddedSubscription(ExchangeConversionRequest request)
        {

        }

        private void OnExistingSubscription(ExchangeConversionRequest request)
        {

        }
    }
}