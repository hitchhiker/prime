using System;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Core.Prices.Latest;

namespace Prime.Core
{
    internal sealed class LatestPriceProviderContext
    {
        internal LatestPriceProviderContext(IPublicPriceSuper provider, Aggregator aggregator)
        {
            Provider = provider;
            Aggregator = aggregator;
            Network = provider?.Network;
        }

        internal readonly Network Network;
        internal TimeSpan PollingSpan { get; set; } = new TimeSpan(0, 0, 15);
        internal IPublicPriceSuper Provider { get; private set; }
        internal Aggregator Aggregator { get; private set; }
    }
}