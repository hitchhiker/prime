using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prime.MessagingServer;

namespace Prime.SocketClient
{
    public class SocketClientContext
    {
        public SocketClientContext(MessagingServer.Server messagingServer, IPEndPoint ipEndPoint)
        {
            MessagingServer = messagingServer;
            IpEndPoint = ipEndPoint;
        }

        public Server MessagingServer { get; set; }

        public IPEndPoint IpEndPoint { get; set; }
    }
}
