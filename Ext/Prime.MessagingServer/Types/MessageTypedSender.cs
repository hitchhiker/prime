using System;
using System.Collections.Concurrent;
using System.Reflection;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;

namespace Prime.MessagingServer.Types
{
    public class MessageTypedSender
    {
        private readonly IMessenger _m;
        private readonly MethodInfo _sendMethod;
        private readonly ConcurrentDictionary<Type, MethodInfo> _generics = new ConcurrentDictionary<Type, MethodInfo>();

        public MessageTypedSender(IMessenger m)
        {
            _m = m;
            _sendMethod = GetType().GetMethod(nameof(Send), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void UnPackSendReceivedMessage(ExternalMessage m)
        {
            var generic = _generics.GetOrAdd(m.Type, t => _sendMethod.MakeGenericMethod(m.Type));
            generic.Invoke(this, new object[] { m });
        }

        private void Send<T>(ExternalMessage externalMessage) where T : BaseTransportMessage
        {
            _m.SendAsync(externalMessage.Message as T);
        }
    }
}