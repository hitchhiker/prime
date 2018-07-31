using System;
using Prime.Core;
using Prime.SocketServer.Transport;

namespace Prime.SocketServer
{
    /// <summary>
    /// Marshalls messages across the internal IMessenger mechanism via the TCP socket server.
    /// </summary>
    public class Server
    {
        public readonly SocketServerContext Context;
        private readonly ServerContext _serverContext;
        private readonly TypedServiceProvider _serviceProvider;

        internal readonly TcpSocketServer TcpSocketServer;

        public readonly ILogger L;

        public Server(SocketServerContext socketServerContext)
        {
            Context = socketServerContext;
            _serverContext = Context.ServerContext;

            _serviceProvider = new TypedServiceProvider(this);
            TcpSocketServer = new TcpSocketServer(_serviceProvider);

            L = socketServerContext?.MessagingServer?.L ?? new NullLogger();
        }
        
        public void Start()
        {
            TcpSocketServer.Start(Context.IpAddress, Context.PortNumber);
        }

        public void Stop()
        {
            _serverContext.M.UnregisterAsync(this);
            TcpSocketServer.ForceStop();
        }
        
        public void Send<T>(T message) where T : BaseTransportMessage
        {
            // BUG: return if message is remote!
            // Spread message to all connected TCP socket clients.
//            if (message.IsRemote)
//            {
//                _serviceProvider.Send(null, message);
//                return;
//            }

            if (message.SessionId.IsNullOrEmpty())
                _serviceProvider.Send(null, message);
            else
                _serviceProvider.Send(message.SessionId, message);
        }
    }
}
