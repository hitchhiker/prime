using System.Collections.Generic;
using Prime.Core;
using Prime.Common;

namespace Prime.Common.Exchange.Rates
{
    internal class ExchangeRateSubscriptions
    {
        public readonly List<ExchangeRateSubscriptions> Requests = new List<ExchangeRateSubscriptions>();
    }
}