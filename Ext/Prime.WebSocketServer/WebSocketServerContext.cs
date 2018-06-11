using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prime.Core;
using Prime.MessagingServer;

namespace Prime.WebSocketServer
{
    public class WebSocketServerContext
    {
        public readonly MessagingServer.Server MessageServer;
        public readonly IPAddress IpAddress;
        public readonly short Port;

        public WebSocketServerContext(MessagingServer.Server server) : this(server, IPAddress.Parse("0.0.0.0"), 9991)
        {
            MessageServer = server;
        }

        public WebSocketServerContext(MessagingServer.Server server, IPAddress ipAddress, short port)
        {
            MessageServer = server;
            IpAddress = ipAddress;
            Port = port;
        }
    }
}
