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

        public static void SendAsync<TMessage>(this IMessenger messenger, TMessage message)
        {
            new Task(() =>
            {
                messenger.Send(message);
            }).Start();
        }

        public static void SendAsync<TMessage>(this IMessenger messenger, TMessage message, object token)
        {
            new Task(() =>
            {
                messenger.Send(message, token);
            }).Start();
        }

        public static void RegisterAsync<TMessage>(this IMessenger messenger, object recipient, object token, Action<TMessage> action)
        {
            void Ka(TMessage m) => RegisterAction(action, m);
            KeepAlive.Add(new Tuple<object, object>(recipient, (Action<TMessage>)Ka));
            messenger.Register<TMessage>(recipient, token, Ka);
        }

        public static void RegisterAsync<TMessage>(this IMessenger messenger, object recipient, Action<TMessage> action)
        {
            void Ka(TMessage m) => RegisterAction(action, m);
            KeepAlive.Add(new Tuple<object, object>(recipient, (Action<TMessage>)Ka));
            messenger.Register<TMessage>(recipient, Ka);
        }

        public static void RegisterAsync<TMessage>(this IMessenger messenger, object recipient, Dispatcher dispatcher, Action<TMessage> action)
        {
            void Ka(TMessage m) => RegisterAction(dispatcher, action, m);
            KeepAlive.Add(new Tuple<object, object>(recipient, (Action<TMessage>)Ka));
            messenger.Register<TMessage>(recipient, Ka);
        }

        private static void RegisterAction<TMessage>(Dispatcher dispatcher, Action<TMessage> action, TMessage t)
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

        private static void RegisterAction<TMessage>(Action<TMessage> action, TMessage t)
        {
            new Task(() =>
            {
                try
                {
                    action(t);
                }
                catch (Exception e)
                {
                    Logging.I.DefaultLogger?.Error(e, "Message exception");
                }
            }).Start();
        }

        public static void UnregisterAsync(this IMessenger messenger, object recipient)
        {
            messenger.Unregister(recipient);
            KeepAlive.RemoveAll(x => x.Item1 == recipient);
        }
    }
}