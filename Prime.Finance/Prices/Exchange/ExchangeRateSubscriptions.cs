using System.Collections.Generic;
using Prime.Core;

namespace Prime.Finance.Exchange.Rates
{
    internal class ExchangeRateSubscriptions
    {
        public readonly List<ExchangeRateSubscriptions> Requests = new List<ExchangeRateSubscriptions>();
    }
}