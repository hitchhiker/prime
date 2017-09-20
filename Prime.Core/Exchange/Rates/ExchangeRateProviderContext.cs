using System;
using Prime.Utility;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRateProviderContext
    {
        public ExchangeRateProviderContext(IPublicPriceProvider provider, ExchangeRatesCoordinator coordinator)
        {
            Provider = provider;
            Coordinator = coordinator;
            Network = provider.Network;
        }

        public readonly Network Network;
        public TimeSpan PollingSpan { get; set; } = new TimeSpan(0, 0, 15);
        public IPublicPriceProvider Provider { get; private set; }
        public ExchangeRatesCoordinator Coordinator { get; private set; }
    }
}