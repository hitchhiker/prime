using System;
using Prime.Core;
using Prime.Core.Testing;

namespace Prime
{
    public class PrimeSocketServer
    {
        public void Start()
        {
            L.Log("Starting tcp server for interproc communication with extensions...");

            StartLocal();
        }

        private void StartLocal()
        {
            var sCtx = new ServerContext()
            {
                L = L
            };
            
            sCtx.Assemblies.Refresh();
            sCtx.Types.Refresh();
            
            var server = new MessagingServer.Server(sCtx);
            server.Start();
            
            var socketServer = new SocketServer.ServerExtension();
            server.Inject(socketServer);
            
            server.M.Register<HelloRequest>(this, (r) =>
            {
                Console.WriteLine("Hello request received.");
                server.M.Send(new HelloResponse(r, "hello"));
            });
            
            socketServer.Start(server);
        }

        public ILogger L { get; set; }
    }
}