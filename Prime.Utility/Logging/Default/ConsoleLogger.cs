using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Common
{
    public class ConsoleLogger : CommonLoggerBase
    {
        public override void Log(LoggingLevel level, string message)
        {
            Console.WriteLine(DateTime.UtcNow.ToLocalTime() + " " + level + " " + message);
        }
    }
}
