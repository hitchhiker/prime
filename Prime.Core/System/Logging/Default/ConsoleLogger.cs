using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public class ConsoleLogger : CommonLoggerBase
    {
        public bool IncludePreamble { get; set; } = true;

        public override void Log(LoggingLevel level, string message)
        {
            if (IncludePreamble)
                Console.WriteLine(DateTime.UtcNow.ToLocalTime() + " " + level + " " + message);
            else
                Console.WriteLine(message);
        }
    }
}
