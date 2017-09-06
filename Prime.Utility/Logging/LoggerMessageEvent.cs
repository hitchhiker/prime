using System;

namespace Prime.Utility
{
    public class LoggerMessageEvent : EventArgs
    {
        public readonly string Key;
        public readonly LoggingLevel Level;
        public readonly string Message;
        public readonly bool SameLine;

        public LoggerMessageEvent(string key, LoggingLevel level, string message, bool sameLine = false)
        {
            Key = key;
            Level = level;
            Message = message;
            SameLine = sameLine;
        }    
    }
}