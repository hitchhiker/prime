using System;
using Prime.Utility;

namespace Prime.Common.Exchange.Rates
{
    public class LatestPriceProviderContext
    {
        public LatestPriceProviderContext(IPublicPriceProvider provider, LatestPriceAggregator aggregator)
        {
            Provider = provider;
            Aggregator = aggregator;
            Network = provider.Network;
        }

        public readonly Network Network;
        public TimeSpan PollingSpan { get; set; } = new TimeSpan(0, 0, 15);
        public IPublicPriceProvider Provider { get; private set; }
        public LatestPriceAggregator Aggregator { get; private set; }
    }
}