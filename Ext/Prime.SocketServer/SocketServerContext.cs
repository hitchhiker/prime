using System.Net;
using Prime.Core;
using Prime.MessagingServer;

namespace Prime.SocketServer
{
    public class SocketServerContext
    {
        public readonly PrimeContext PrimeContext;
        public readonly MessagingServer.Server MessagingServer;
        public readonly IPAddress IpAddress = IPAddress.Any;
        public readonly short PortNumber = 9991;

        public const string LogServerName = "Socket server";

        public SocketServerContext(MessagingServer.Server server, IPAddress ipAddress, short portNumber) : this(server)
        {
            IpAddress = ipAddress;
            PortNumber = portNumber;
        }

        public SocketServerContext(MessagingServer.Server server)
        {
            MessagingServer = server;
            PrimeContext = server.PrimeContext;
        }
    }
}