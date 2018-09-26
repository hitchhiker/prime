using System;

namespace Prime.Core
{
    public static class LoggerExtensions
    {
        public static void Log(this ILogger logger, string message, LoggingLevel level = LoggingLevel.Status)
        {
            logger.Log(message, level);
        }

        public static void Trace(this ILogger logger, string message)
        {
            Log(logger, LoggingLevel.Trace, message);
        }

        public static void Info(this ILogger logger, string message)
        {
            Log(logger, LoggingLevel.Status, message);
        }

        public static void Warn(this ILogger logger, string message)
        {
            Log(logger, LoggingLevel.Warning, message);
        }

        public static void Error(this ILogger logger, string message)
        {
            Log(logger, LoggingLevel.Error, message);
        }

        public static void Error(this ILogger logger, Exception e)
        {
            Log(logger, e.Message);
        }

        public static void Error(this ILogger logger, Exception e, string message)
        {
            Log(logger, e.Message + ": " + message);
        }

        public static void Log(this ILogger logger, LoggingLevel level, string message)
        {
            Log(logger, message, level);
        }

        public static void Fatal(this ILogger logger, string message)
        {
            Log(logger, LoggingLevel.Panic, message);
        }

        public static void Log(this ILogger logger, string message, params object[] parameters)
        {
            logger.Log(message, LoggingLevel.Trace, parameters);
        }

        public static void Trace(this ILogger logger, string message, params object[] parameters)
        {
            logger.Log(message, LoggingLevel.Trace, parameters);
        }

        public static void Info(this ILogger logger, string message, params object[] parameters)
        {
            logger.Log(message, LoggingLevel.Status, parameters);
        }

        public static void Warn(this ILogger logger, string message, params object[] parameters)
        {
            logger.Log(message, LoggingLevel.Warning, parameters);
        }

        public static void Error(this ILogger logger, string message, params object[] parameters)
        {
            logger.Log(message, LoggingLevel.Error, parameters);
        }

        public static void Fatal(this ILogger logger, string message, params object[] parameters)
        {
            logger.Log(message, LoggingLevel.Panic, parameters);
        }

        public static void Log(this ILogger logger, object obj, LoggingLevel level = LoggingLevel.Status)
        {
            logger.Log(obj.ToString(), level);
        }

        public static void Info(this ILogger logger, object obj)
        {
            logger.Log(obj.ToString(), LoggingLevel.Status);
        }
    }
}