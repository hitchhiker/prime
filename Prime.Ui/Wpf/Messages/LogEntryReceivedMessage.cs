using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class LogEntryReceivedMessage : MessageBase
    {
        public LogEntryReceivedMessage(DateTime utcCreated, string key, string message, LoggingLevel logLevel, bool sameLine = false)
        {
            Key = key;
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            UtcCreated = utcCreated;
            Message = message;
            LogLevel = logLevel;
            SameLine = sameLine;
        }

        public LogEntryReceivedMessage(LoggerMessageEvent me)
        {
            UtcCreated = DateTime.UtcNow;
            Key = me.Key;
            Message = me.Message;
            LogLevel = me.Level;
            SameLine = me.SameLine;
        }

        public readonly bool SameLine;
        public readonly string Key;
        public readonly DateTime UtcCreated;
        public readonly string Message;
        public readonly LoggingLevel LogLevel;
    }
}