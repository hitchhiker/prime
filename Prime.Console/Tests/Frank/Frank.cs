﻿using System;
using System.Collections.Generic;
using Prime.Core.Authentication;
using Prime.Core;

namespace Prime.ConsoleApp.Tests.Frank
{
    public class Frank
    {
        public static void Go()
        {
            var logger = new ConsoleLogger();
            AuthManagerTest.Go(logger);
        }
    }
}
