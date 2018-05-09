using System;

namespace Prime.Core
{
    public interface ILogger
    {
        void Log(string message, LoggingLevel level);

        void Log(string message, LoggingLevel level, params object[] parameters);

        T Trace<T>(string messagePrefix, Func<T> action);
    }
}