using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Core.Exchange.Rates;

namespace Prime.Core
{
    public sealed class PrimeServerInstance : ICore
    {
        public readonly PrimeContext PrimeContext;

        public PrimeServerInstance() : this(new PrimeContext(DefaultMessenger.I.Default)) {}

        public PrimeServerInstance(PrimeContext primeContext)
        {
            PrimeContext = primeContext;
            StartupMessengers = TypeCatalogue.I.ImplementInstances<IStartupMessenger>().ToList();
            Environment = TypeCatalogue.I.ImplementInstances<IPrimeEnvironment>().FirstOrDefault();

            if (Environment == null)
                throw new Exception($"Cannot find instance of {nameof(IPrimeEnvironment)}. This is probably due to an installation issue.");
        }

        public readonly List<IStartupMessenger> StartupMessengers;

        public readonly IPrimeEnvironment Environment;
        
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
