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

        public void SendData(string data)
        {
            // TODO: re-implement using clientIds (sessionIds).
            Sessions.Broadcast(data);
        }

        private ObjectId _sessionId;
        public ObjectId SessionId => _sessionId ?? (_sessionId = ID.GetObjectIdHashCode());
    }
}
