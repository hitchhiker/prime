using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Common
{
    public class MessengerLogger : CommonLoggerBase
    {
        public readonly string Key;
        public readonly Action<string> StatusConsumer;
        public readonly IMessenger Messenger = DefaultMessenger.I.Default;

        public MessengerLogger(string key = null)
        {
            Key = key;
        }

        public MessengerLogger(Action<string> statusConsumer, string key = null) : this(key)
        {
            StatusConsumer = statusConsumer;
        }

        public void Status(string message)
        {
            StatusConsumer?.Invoke(message);
        }

        public override void Log(LoggingLevel level, string message)
        {
            Messenger.Send(new NewLogMessage(DateTime.UtcNow, message, level), Key);
        }
    }
}
