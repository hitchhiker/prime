using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public static class MessengerExtensionMethods
    {
        public static List<Tuple<object, object>> KeepAlive = new List<Tuple<object, object>>();

        public static void SendAsync<TMessage>(this IMessenger messenger, TMessage message)
        {
            Task.Run(() => { messenger.Send(message); });
        }

        public static void SendAsync<TMessage>(this IMessenger messenger, TMessage message, object token)
        {
            Task.Run(() => { messenger.Send(message, token); });
        }

        public static void RegisterAsync<TMessage>(this IMessenger messenger, object recipient, object token, Action<TMessage> action)
        {
            void Ka(TMessage m) => RegisterAction(action, m);
            KeepAlive.Add(new Tuple<object, object>(recipient, (Action<TMessage>)Ka));
            messenger.Register<TMessage>(recipient, token, Ka);
        }

        public static void RegisterAsync<TMessage>(this IMessenger messenger, object recipient, Action<TMessage> action, bool derivedMessagesToo = false)
        {
            void Ka(TMessage m) => RegisterAction(action, m);
            KeepAlive.Add(new Tuple<object, object>(recipient, (Action<TMessage>)Ka));
            messenger.Register<TMessage>(recipient, derivedMessagesToo, Ka);
        }

        private static void RegisterAction<TMessage>(Action<TMessage> action, TMessage t)
        {
            Task.Run(() =>
            {
                try
                {
                    action(t);
                }
                catch (Exception e)
                {
                    Logging.I.DefaultLogger?.Error(e, "Message exception");
                }
            });
        }

        public static void UnregisterAsync(this IMessenger messenger, object recipient)
        {
            messenger.Unregister(recipient);
            KeepAlive.RemoveAll(x => x.Item1 == recipient);
        }

        
        public static TReply SendAndWait<TSend, TReply>(this IMessenger m, TSend requestMessage, Func<TReply, bool> messageCheck = null) 
            where TSend : BaseTransportRequestMessage 
            where TReply : BaseTransportResponseMessage
        {
            var registrationobj = new object();

            TReply r = default;

            m.Register<TReply>(registrationobj, msg =>
            {
                r = msg;
            });

            m.Send(requestMessage);

            do
            {
                Thread.Sleep(1);
            } while (Equals(r, default) || r.RequestId!=requestMessage.RequestId || messageCheck != null && messageCheck(r)==false);

            m.Unregister(registrationobj);
            return r;
        }
    }
}