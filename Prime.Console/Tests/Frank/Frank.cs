using System;
using System.Collections.Generic;
using Prime.Core.Authentication;
using Prime.Core;
using Prime.Extensions;

namespace Prime.ConsoleApp.Tests.Frank
{
    public class Frank
    {
        public static void Go()
        {
            var logger = new ConsoleLogger();

            //AuthManagerTest.Go(logger);

            new ExtensionLoader().Compose();
        }
    }
}
