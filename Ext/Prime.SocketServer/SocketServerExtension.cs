using System;
using System.Collections.Generic;
using System.Text;
using Prime.Base;
using Prime.Core;
using Prime.MessagingServer;

namespace Prime.SocketServer
{
    public class SocketServerExtension : IMessageServerExtension
    {
        private static readonly ObjectId _id = "prime:socketserver".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime Socket Server";

        public Version Version { get; } = new Version("1.0.0");

        internal SocketServer SocketServerInstance;

        public void Start(Server server)
        {
            SocketServerInstance = new SocketServer(new SocketServerContext(server));
            SocketServerInstance.Start();
        }

        public void Stop()
        {
            SocketServerInstance.Stop();
            SocketServerInstance = null;
        }

        public void Send(BaseTransportMessage message)
        {
            SocketServerInstance?.Send(message);
        }
    }
}
