using System;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Common
{
    public class NewLogMessage : MessageBase
    {
        public NewLogMessage(DateTime utcCreated, string message, LoggingLevel logLevel, bool sameLine = false)
        {
            UtcCreated = utcCreated;
            Message = message ?? string.Empty;
            LogLevel = logLevel;
            SameLine = sameLine;
        }
        
        public readonly bool SameLine;
        public readonly DateTime UtcCreated;
        public readonly string Message;
        public readonly LoggingLevel LogLevel;
    }
}