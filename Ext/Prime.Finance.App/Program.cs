using System;
using Prime.Core;

namespace Prime.Finance.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger() { IncludePreamble = true };

            logger.Log("Operating system: " + Environment.OSVersion.Platform);
            logger.Log("Current directory: " + Environment.CurrentDirectory);

            logger.Log("Prime.Finance started");
        }
    }
}
