using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using Prime.Base;
using Prime.Core;
using Prime.MessagingServer;

namespace Prime.WebSocketServer
{
    [Export(typeof(IExtension))]
    public class ServerExtension : IMessageServerExtension
    {
        private static readonly ObjectId _id = "prime:websocketserver".GetObjectIdHashCode();
        public ObjectId Id => _id;
        public string Title { get; } = "Prime WebSocket Server";
        public Version Version { get; } = new Version("1.0.0");

        private Server _webSocketServerInstance;

        public void Start(MessagingServer.Server server)
        {
            _webSocketServerInstance = new Server(new WebSocketServerContext(server));
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
