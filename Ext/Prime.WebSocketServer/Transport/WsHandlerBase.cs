using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using Prime.Base;
using Prime.Core;
using WebSocketSharp.Server;

namespace Prime.WebSocketServer.Transport
{
    public abstract class WsHandlerBase : WebSocketBehavior
    {
        public IMessenger M;
        public ILogger L;
        public MessagingServer MessageServer;

        public void SendTo(string data, string id)
        {
            // TODO: re-implement using clientIds (sessionIds). HACK
            Broadcast(data);

            //Sessions.SendTo(data, id);
        }

        public void Broadcast(string data)
        {
            Sessions.Broadcast(data);
        }

        private ObjectId _sessionId;
        public ObjectId SessionId => _sessionId ?? (_sessionId = ID.GetObjectIdHashCode());
    }
}
