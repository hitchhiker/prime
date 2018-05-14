using System;
using System.Collections.Generic;
using System.Text;
using Prime.Base;
using Prime.Core;
using Prime.MessagingServer;

namespace Prime.WebSocketServer
{
    public class WsServerExtension : IMessageServerExtension
    {
        private static readonly ObjectId _id = "prime:websocketserver".GetObjectIdHashCode();
        public ObjectId Id => _id;
        public string Title { get; } = "Prime WebSocket Server";
        public Version Version { get; } = new Version("1.0.0");

        private WsServer _webSocketServerInstance;

        public void Start(Server server)
        {
            _webSocketServerInstance = new WsServer(new WsServerContext(server));
            _webSocketServerInstance.Start();
        }

        public void Stop()
        {
            _webSocketServerInstance.Stop();
            _webSocketServerInstance = null;
        }

        public void Send(BaseTransportMessage message)
        {
            _webSocketServerInstance?.Send(message);
        }
    }
}
