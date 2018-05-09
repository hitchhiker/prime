using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public abstract class CommonLoggerBase : ILogger
    {
        public void Log(string message, LoggingLevel level = LoggingLevel.Status)
        {
            Log(level, message);
        }

        public void Log(string message, LoggingLevel level, params object[] parameters)
        {
            Log(string.Format(message, parameters), level);
        }

        public void Trace(string message)
        {
            Log(LoggingLevel.Trace, message);
        }

        public T Trace<T>(string messagePrefix, Func<T> action)
        {
            var sw = new Stopwatch();
            sw.Start();
            Trace("Begin: " + messagePrefix);
            var r = action();
            Trace("End: " + messagePrefix + ": " + sw.ToElapsed());
            return r;
        }

        public abstract void Log(LoggingLevel level, string message);
    }
}