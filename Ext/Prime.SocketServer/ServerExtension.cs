using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using Prime.Base;
using Prime.Core;
using Prime.MessagingServer;

namespace Prime.SocketServer
{
    [Export(typeof(IExtension))]
    public class ServerExtension : IMessageServerExtension
    {
        private static readonly ObjectId _id = "prime:socketserver".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime Socket Server";

        public Version Version { get; } = new Version("1.0.0");

        internal Server ServerInstance;

        public void Start(MessagingServer.Server server)
        {
            ServerInstance = new Server(new SocketServerContext(server));
            ServerInstance.Start();
        }

        public void Stop()
        {
            ServerInstance.Stop();
            ServerInstance = null;
        }

        public void Send(BaseTransportMessage message)
        {
            ServerInstance?.Send(message);
        }
    }
}
