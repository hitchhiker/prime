using System;

namespace Prime.Utility
{
    public class Logging
    {
        private Logging()
        {
            Common = new Logger("system");
        }

        public static Logging I => Lazy.Value;
        private static readonly Lazy<Logging> Lazy = new Lazy<Logging>(()=>new Logging());

        public void SendMessage(Logger logger, LoggingLevel level, string message)
        {
            OnNewMessage?.Invoke(logger, new LoggerMessageEvent(logger.Key, level, message));
        }

        public event EventHandler OnNewMessage;

        public Logger Common { get; private set; }
    }
}