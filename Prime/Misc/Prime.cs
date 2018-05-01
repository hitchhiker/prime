using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Core.Exchange.Rates;

namespace Prime.Core
{
    public sealed class Prime : ICore
    {
        private Prime()
        {
            StartupMessengers = TypeCatalogue.I.ImplementInstances<IStartupMessenger>().ToList();
            Environment = TypeCatalogue.I.ImplementInstances<IPrimeEnvironment>().FirstOrDefault();
            if (Environment == null)
                throw new Exception($"Cannot find instance of {nameof(IPrimeEnvironment)}. This is probably due to an installation issue.");
        }

        public static Prime I => Lazy.Value;
        private static readonly Lazy<Prime> Lazy = new Lazy<Prime>(() => new Prime());

        public readonly List<IStartupMessenger> StartupMessengers;

        public readonly IPrimeEnvironment Environment;

        public IWindowManager GetWindowManager(UserContext context)
        {
            return TypeCatalogue.I.ImplementInstancesWith<IWindowManager>(context).FirstOrDefault();
        }

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
