using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Utility;

namespace Prime.Core
{
    public class Prime
    {
        private Prime()
        {
            StartupMessengers = TypeCatalogue.I.ImplementInstances<IStartupMessenger>().ToList();
            LatestPriceAggregator = StartupMessengers.OfType<LatestPriceMessenger>().FirstOrDefault()?.Aggregator;
        }

        public static Prime I => Lazy.Value;
        private static readonly Lazy<Prime> Lazy = new Lazy<Prime>(() => new Prime());

        public readonly List<IStartupMessenger> StartupMessengers;

        public readonly LatestPriceAggregator LatestPriceAggregator;
    }
}
