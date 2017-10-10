using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeConversionCoordinator
    {
        private readonly LatestPriceCoordinator _c;
        private readonly List<ExchangeConversionRequest> _requests = new List<ExchangeConversionRequest>();

        private ExchangeConversionCoordinator()
        {
            _c = LatestPriceCoordinator.I;
        }

        public static ExchangeConversionCoordinator I => Lazy.Value;
        private static readonly Lazy<ExchangeConversionCoordinator> Lazy = new Lazy<ExchangeConversionCoordinator>(()=>new ExchangeConversionCoordinator());

        public void Register(ExchangeConversionRequest request)
        {
            _requests.Add(request);
            request.Register();
        }

        public void Unregister(ExchangeConversionRequest request)
        {
            var e = _requests.FirstOrDefault(x=>x.Match(request));
            if (e != null)
                _requests.Remove(e);
        }
    }
}