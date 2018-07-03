using System.Net;

namespace Prime.WebSocketServer
{
    public class WebSocketServerContext
    {
        public readonly MessagingServer.Server MessageServer;
        public readonly IPAddress IpAddress;
        public readonly short Port;

        public const string LogServerName = "WebSocket server";

        public WebSocketServerContext(MessagingServer.Server server) : this(server, IPAddress.Any, 9992)
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
