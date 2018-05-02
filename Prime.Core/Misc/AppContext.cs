using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public class AppContext
    {
        public AppContext(IMessenger messenger, ILogger logger)
        {
            Messenger = messenger;
            Logger = logger;
        }

        public readonly IMessenger Messenger;
        public readonly ILogger Logger;
    }
}