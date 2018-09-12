using System;
using System.Threading.Tasks;
using Prime.Core;
using Prime.MessagingServer;
using Prime.Settings;

namespace Prime.Web
{
    public static class TcpBridge
    {
        public static void RunClient()
        {
            Task.Run(() => RunClientInternal());
        }

        private static void RunClientInternal()
        {
            var logger = new ConsoleLogger();

            var serverContext = new ServerContext()
            {
                L = logger
            };
            var messagingServer = new Server(serverContext);

            messagingServer.M.Register<TimeKindUiUpdatedMessage>(messagingServer, (request) =>
            {
                TimeKindUpdated?.Invoke(request);
            });

            var clientExtension = new SocketClient.ServerExtension();
            messagingServer.Inject(clientExtension);
            
            messagingServer.Start();
        }

        public static event Action<TimeKindUiUpdatedMessage> TimeKindUpdated;
    }
}
