using System;
using Prime.Base;
using Prime.Core;

namespace Prime.Core
{
    public class ExternalMessage
    {
        public readonly ObjectId SessionId;
        public readonly BaseTransportMessage Message;

        public ExternalMessage(ObjectId sessionId, BaseTransportMessage message)
        {
            SessionId = sessionId;
            Message = message;

            if (message == null)
                return;

            message.SessionId = sessionId;
            message.IsRemote = true;
        }

        private Type _type;
        public Type Type => _type ?? (_type = Message.GetType());
    }
}