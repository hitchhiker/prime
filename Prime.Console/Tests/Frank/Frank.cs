using System;
using System.Collections.Generic;
using Prime.Core.Authentication;
using Prime.Core;
using Prime.Extensions;

namespace Prime.ConsoleApp.Tests.Frank
{
    public class Frank
    {
        public static void Go(PrimeContext context)
        {
            //AuthManagerTest.Go(logger);

            //new ExtensionLoader().Compose();

            var pm = new PackageCoordinator(context);

            pm.EnsureInstalled();

            context.Logger.Info(pm.Distribution.Count);
        }
    }
}
