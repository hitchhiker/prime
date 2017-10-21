using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Utility
{
    public static class MessengerExtensionMethods
    {
        public static List<Tuple<object, object>> KeepAlive = new List<Tuple<object, object>>();

        public static void RegisterAsyncD<TMessage>(this IMessenger messenger, object recipient, object token, Dispatcher dispatcher, Action<TMessage> action)
        {
            void Ka(TMessage m) => RegisterActionD(dispatcher, action, m);
            KeepAlive.Add(new Tuple<object, object>(recipient, (Action<TMessage>)Ka));
            messenger.Register<TMessage>(recipient, token, Ka);
        }

        public static void RegisterAsyncD<TMessage>(this IMessenger messenger, object recipient, Dispatcher dispatcher, Action<TMessage> action)
        {
            void Ka(TMessage m) => RegisterActionD(dispatcher, action, m);
            KeepAlive.Add(new Tuple<object, object>(recipient, (Action<TMessage>)Ka));
            messenger.Register<TMessage>(recipient, Ka);
        }

        private static void RegisterActionD<TMessage>(Dispatcher dispatcher, Action<TMessage> action, TMessage t)
        {
            new Task(() =>
            {
                try
                {
                    dispatcher.Invoke(() =>
                    {
                        action(t);
                    });
                }
                catch (Exception e)
                {
                    Logging.I.DefaultLogger?.Error(e, "Message exception");
                }
            }).Start();
        }

        public static void UnregisterD(this IMessenger messenger, object recipient)
        {
            messenger.Unregister(recipient);
            KeepAlive.RemoveAll(x => x.Item1 == recipient);
            messenger.UnregisterAsync(recipient);
        }
    }
}