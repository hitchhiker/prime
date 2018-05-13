using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prime.Core;

namespace Prime.WebSocketServer
{
    public class WsServerContext
    {
        public readonly MessageServer MessageServer;
        public readonly IPAddress IpAddress;
        public readonly short Port;

        public WsServerContext(MessageServer server) : this(server, IPAddress.Any, 9991)
        {
            MessageServer = server;
        }

        public WsServerContext(MessageServer server, IPAddress ipAddress, short port)
        {
            MessageServer = server;
            IpAddress = ipAddress;
            Port = port;
        }
    }
}
