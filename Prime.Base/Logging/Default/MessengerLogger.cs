using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public class MessengerLogger : CommonLoggerBase
    {
        public readonly string Key;
        public readonly Action<string> StatusConsumer;
        public readonly IMessenger Messenger;

        public MessengerLogger(IMessenger messenger, string key = null)
        {
            Messenger = messenger;
            Key = key;
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
