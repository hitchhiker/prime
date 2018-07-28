using System;
using Prime.Base.Messaging.Manager;
using Prime.Core;
using Prime.Core.Testing;

namespace Prime
{
    public class PrimeSocketServer
    {
        private readonly ServerContext _serverContext;
        private readonly MessagingServer.Server _messagingServer;

        public PrimeSocketServer(ServerContext serverContext, MessagingServer.Server messagingServer)
        {
            _serverContext = serverContext;
            _messagingServer = messagingServer;
        }

        public void Start()
        {
            L.Log("Starting TCP server for interproc communication with extensions...");

            StartLocal();
        }

        private void StartLocal()
        {
           
            var socketServer = new SocketServer.ServerExtension();
            _messagingServer.Inject(socketServer);

            _messagingServer.M.Register<HelloRequest>(this, (r) =>
            {
                Console.WriteLine("Hello request received.");

                var response = new HelloResponse(r, "hello");

                _messagingServer.M.Send(response);
            });

            _messagingServer.M.Register<TimeKindUpdatedRequestMessage>(this, (r) =>
            {
                Console.WriteLine("TimeKindUpdatedRequestMessage request received.");
            });
            
            //socketServer.Start(_messagingServer);
        }

        public ILogger L { get; set; }
    }
}