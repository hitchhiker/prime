using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Core;
using Prime.Core.Exchange.Rates;
using Prime.Core.Prices.Latest;

namespace Prime.Core
{
    internal sealed class Prime : ICore
    {
        private Prime()
        {
            StartupMessengers = TypeCatalogue.I.ImplementInstances<IStartupMessenger>().ToList();
            Aggregator = StartupMessengers.OfType<Messenger>().FirstOrDefault()?.Aggregator;
        }

        internal static Prime I => Lazy.Value;
        private static readonly Lazy<Prime> Lazy = new Lazy<Prime>(() => new Prime());

        internal readonly List<IStartupMessenger> StartupMessengers;

        internal readonly Aggregator Aggregator;

        public void Start()
        {
           //
        }

        public void Stop()
        {
            //
        }
    }
}
