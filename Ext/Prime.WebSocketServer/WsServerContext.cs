using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prime.Core;
using Prime.MessagingServer;

namespace Prime.WebSocketServer
{
    public class WsServerContext
    {
        public readonly Server MessageServer;
        public readonly IPAddress IpAddress;
        public readonly short Port;

        public WsServerContext(Server server) : this(server, IPAddress.Any, 9991)
        {
            MessageServer = server;
        }

        public WsServerContext(Server server, IPAddress ipAddress, short port)
        {
            MessageServer = server;
            IpAddress = ipAddress;
            Port = port;
        }
    }
}
