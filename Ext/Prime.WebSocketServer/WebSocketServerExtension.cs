using System;
using System.Collections.Generic;
using System.Text;
using Prime.Base;
using Prime.Core;

namespace Prime.WebSocketServer
{
    public class WebSocketServerExtension : IMessageServerExtension
    {
        private static readonly ObjectId _id = "prime:websocketserver".GetObjectIdHashCode();
        public ObjectId Id => _id;
        public string Title { get; } = "Prime WebSocket Server";
        public Version Version { get; } = new Version("1.0.0");

        public void Start(MessageServer server)
        {
            server.L.Log("Ws started");
        }

        public void Stop()
        {
            
        }

        public void Send(BaseTransportMessage message)
        {
            
        }
    }
}
