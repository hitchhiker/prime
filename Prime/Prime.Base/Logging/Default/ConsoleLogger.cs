using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public class ConsoleLogger : CommonLoggerBase
    {
        public bool IncludePreamble { get; set; } = true;
        public bool RepeatTime { get; set; } = true;
        public string ColumnString { get; set; } = "{0,-22}{1,-50}";

        private volatile string _lastTime;
        
        public override void Log(LoggingLevel level, string message)
        {
            if (!IncludePreamble)
            {
                Console.WriteLine(message);
                return;
            }

            var t = DateTime.UtcNow.ToLocalTime().ToString();
            if (t == _lastTime && !RepeatTime)
                t = "";
            else
                _lastTime = t;

            var oc = Console.ForegroundColor;

            switch (level)
            {
                case LoggingLevel.Status:
                case LoggingLevel.Trace:
                    Console.WriteLine(ColumnString, t, message);
                    break;
                case LoggingLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(ColumnString, t, $"Warn: {message}");
                    break;
                case LoggingLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ColumnString, t, $"ERROR: {message}");
                    break;
                case LoggingLevel.Panic:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ColumnString, t, $"FATAL: {message}");
                    break;
            }

            Console.ForegroundColor = oc;
        }
    }
}
