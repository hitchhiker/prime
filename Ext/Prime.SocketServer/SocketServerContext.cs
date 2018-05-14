﻿using System.Net;
using Prime.Core;
using Prime.MessagingServer;

namespace Prime.SocketServer
{
    public class SocketServerContext
    {
        public readonly ServerContext ServerContext;
        public readonly Server MessagingServer;
        public readonly IPAddress IpAddress = IPAddress.Any;
        public readonly short PortNumber = 19991;

        public SocketServerContext(Server server, IPAddress ipAddress, short portNumber) : this(server)
        {
            
            IpAddress = ipAddress;
            PortNumber = portNumber;
        }

        public SocketServerContext(Server server)
        {
            MessagingServer = server;
            ServerContext = server.ServerContext;
        }
    }
}