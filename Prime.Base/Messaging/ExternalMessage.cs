using System;
using Prime.Base;
using Prime.Core;

namespace Prime.Core
{
    public class ExternalMessage
    {
        public readonly ObjectId ClientId;
        public readonly BaseTransportMessage Message;

        public ExternalMessage(ObjectId clientId, BaseTransportMessage message)
        {
            ClientId = clientId;
            Message = message;

            if (message == null)
                return;

            message.ClientId = clientId;
            message.IsRemote = true;
        }

        private Type _type;
        public Type Type => _type ?? (_type = Message.GetType());
    }
}