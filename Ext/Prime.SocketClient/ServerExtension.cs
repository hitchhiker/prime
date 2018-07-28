using System;
using System.Collections.Generic;
using System.Composition;
using System.Net;
using System.Text;
using Prime.Base;
using Prime.Core;
using Prime.MessagingServer;

namespace Prime.SocketClient
{
    [Export(typeof(IExtension))]
    public class ServerExtension : IMessageServerExtension
    {
        private static readonly ObjectId _id = "prime:socketclient".GetObjectIdHashCode();
        public ObjectId Id => _id;
        public string Title { get; } = "Primce Socket Client";
        public Version Version { get; } = new Version("1.0.0");

        internal Client ClientInstance;

        public void Start(Server server)
        {
            ClientInstance = new Client(new SocketClientContext(server, new IPEndPoint(IPAddress.Loopback, 9991)));
            ClientInstance.Start();
        }

        public void Stop()
        {
            ClientInstance.Stop();
            ClientInstance = null;
        }

        public void Send(BaseTransportMessage message)
        {
            ClientInstance?.Send(message);
        }
    }
}
