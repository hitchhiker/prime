using System.Net;
using Prime.Core;

namespace Prime.SocketServer
{
    public class SocketServerContext
    {
        public readonly ServerContext ServerContext;
        public readonly MessageServer MessageServer;
        public readonly IPAddress IpAddress = IPAddress.Any;
        public readonly short PortNumber = 19991;

        public SocketServerContext(MessageServer server, IPAddress ipAddress, short portNumber) : this(server)
        {
            IpAddress = ipAddress;
            PortNumber = portNumber;
        }

        public SocketServerContext(MessageServer server)
        {
            MessageServer = server;
            ServerContext = server.ServerContext;
        }
    }
}