using System.Collections.Generic;
using Prime.Core;

namespace Prime.Core.Exchange.Rates
{
    internal class ExchangeRateSubscriptions
    {
        public readonly List<ExchangeRateSubscriptions> Requests = new List<ExchangeRateSubscriptions>();
    }
}