using System;
using Prime.Core;

namespace Prime.SocketServer
{
    /// <summary>
    /// Marshalls messages across the internal IMessenger mechanism via the TCP socket server.
    /// </summary>
    public class Server
    {
        public readonly SocketServerContext Context;
        private readonly ServerContext _serverContext;
        internal readonly TcpServer TcpServer;

        public readonly ILogger L;

        public Server(SocketServerContext socketServerContext)
        {
            Context = socketServerContext;
            _serverContext = Context.ServerContext;
            TcpServer = new TcpServer(this);

            L = socketServerContext?.MessagingServer?.L ?? new NullLogger();
        }
        
        public void Start()
        {
            TcpServer.ExceptionOccurred += TcpServerOnExceptionOccurred;
            TcpServer.Start(Context.IpAddress, Context.PortNumber);
        }

        public void Stop()
        {
            _serverContext.M.UnregisterAsync(this);
            TcpServer.Stop();
        }
        
        public void Send<T>(T message) where T : BaseTransportMessage
        {
            if (message.IsRemote)
                return;

            if (message.SessionId.IsNullOrEmpty())
                TcpServer.Send(null, message);
            else
            {
                var client = TcpServer.GetClient(message.SessionId);
                if (client == null)
                    _serverContext.L.Error("Socket server: ClientID " + message.SessionId + " is not a registered client.");
                else
                    TcpServer.Send(client, message);
            }
        }

        private void TcpServerOnExceptionOccurred(object sender, Exception exception)
        {
            Log($"Server error occurred: {exception.Message}");
        }

        private void Log(string text)
        {
            L.Log($"({typeof(Server).Name}) : {text}");
        }
    }
}
