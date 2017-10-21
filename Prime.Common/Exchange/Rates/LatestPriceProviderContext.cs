using System;
using Prime.Utility;

namespace Prime.Common.Exchange.Rates
{
    public class LatestPriceProviderContext
    {
        public LatestPriceProviderContext(IPublicPriceProvider provider, LatestPriceCoordinator coordinator)
        {
            Provider = provider;
            Coordinator = coordinator;
            Network = provider.Network;
        }

        public readonly Network Network;
        public TimeSpan PollingSpan { get; set; } = new TimeSpan(0, 0, 15);
        public IPublicPriceProvider Provider { get; private set; }
        public LatestPriceCoordinator Coordinator { get; private set; }
    }
}