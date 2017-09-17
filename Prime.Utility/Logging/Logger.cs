using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Utility
{
    public class Logger
    {
        public readonly string Key;
        public readonly Action<string> StatusConsumer;

        public Logger(string key = null)
        {
            Key = key ?? "system";
        }

        public Logger(Action<string> statusConsumer, string key = null) : this(key)
        {
            StatusConsumer = statusConsumer;
        }

        public void Log(string message, LoggingLevel level = LoggingLevel.Status)
        {
            Info(message);
        }

        public void Log(string message, LoggingLevel level, params object[] parameters)
        {
            Log(string.Format(message, parameters), level);
        }

        public void Trace(string message)
        {
            Logging.I.SendMessage(this, LoggingLevel.Trace, message);
        }

        public void Info(string message)
        {
            Logging.I.SendMessage(this, LoggingLevel.Status, message);
        }

        public void Warn(string message)
        {
            Logging.I.SendMessage(this, LoggingLevel.Warning, message);
        }

        public void Error(string message)
        {
            Logging.I.SendMessage(this, LoggingLevel.Error, message);
        }

        public void Fatal(string message)
        {
            Logging.I.SendMessage(this, LoggingLevel.Panic, message);
        }

        public void Trace(string message, params object[] parameters)
        {
            Log(message, LoggingLevel.Trace, parameters);
        }

        public void Info(string message, params object[] parameters)
        {
            Log(message, LoggingLevel.Status, parameters);
        }

        public void Warn(string message, params object[] parameters)
        {
            Log(message, LoggingLevel.Warning, parameters);
        }

        public void Error(string message, params object[] parameters)
        {
            Log(message, LoggingLevel.Error, parameters);
        }

        public void Fatal(string message, params object[] parameters)
        {
            Log(message, LoggingLevel.Panic, parameters);
        }

        public void Error(Exception e)
        {
            Log(e.Message);
        }

        public void Error(Exception e, string message)
        {
            Log(e.Message + ": " + message);
        }

        public void Status(string message)
        {
            StatusConsumer?.Invoke(message);
        }

        public T Trace<T>(string messagePrefix, Func<T> action)
        {
            var sw = new Stopwatch();
            sw.Start();
            Trace("Begin: " + messagePrefix);
            var r= action();
            Trace("End: " + messagePrefix + ": " + sw.ToElapsed());
            return r;
        }
    }
}
