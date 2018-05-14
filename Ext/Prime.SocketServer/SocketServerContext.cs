using System.Net;
using Prime.Core;

namespace Prime.SocketServer
{
    public class SocketServerContext
    {
        public readonly ServerContext ServerContext;
        public readonly MessagingServer MessagingServer;
        public readonly IPAddress IpAddress = IPAddress.Any;
        public readonly short PortNumber = 19991;

        public SocketServerContext(MessagingServer server, IPAddress ipAddress, short portNumber) : this(server)
        {
            
            IpAddress = ipAddress;
            PortNumber = portNumber;
        }

        public SocketServerContext(MessagingServer server)
        {
            MessagingServer = server;
            ServerContext = server.ServerContext;
        }
    }
}