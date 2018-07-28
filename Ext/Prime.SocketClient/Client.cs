using Prime.Core;
using Prime.SocketClient.Transport;

namespace Prime.SocketClient
{
    public class Client
    {
        private readonly TcpSocketClient _socketClient;
        private readonly SocketClientContext _context;

        public Client(SocketClientContext context)
        {
            _context = context;
            _socketClient = new TcpSocketClient(context.MessagingServer)
            {
                L = _context.MessagingServer.L
            };
        }

        public void Start()
        {
            _socketClient.Connect(_context.IpEndPoint);
        }

        public void Stop()
        {
            _socketClient.Disconnect();
        }

        public void Send(BaseTransportMessage message)
        {
            _socketClient.Send(message);
        }
    }
}
