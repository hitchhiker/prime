using System;

namespace Prime.Utility
{
    public interface ILogger
    {
        void Log(string message, LoggingLevel level = LoggingLevel.Status);
        void Log(string message, LoggingLevel level, params object[] parameters);
        void Trace(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
        void Trace(string message, params object[] parameters);
        void Info(string message, params object[] parameters);
        void Warn(string message, params object[] parameters);
        void Error(string message, params object[] parameters);
        void Fatal(string message, params object[] parameters);
        void Error(Exception e);
        void Error(Exception e, string message);
        void Status(string message);
        T Trace<T>(string messagePrefix, Func<T> action);
        void Log(LoggingLevel level, string message);
    }
}