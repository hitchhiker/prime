using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prime.Base.Messaging.Manager;
using Prime.Core;
using Prime.MessagingServer;
using Prime.SocketServer.Transport;

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

            messagingServer.M.Register<TimeKindUpdatedRequestMessage>(messagingServer, (request) =>
            {
                TimeKindUpdated?.Invoke(request);
            });

            var tcpClient = new TcpSocketClient(messagingServer)
            {
                L = logger
            };
            tcpClient.Connect("127.0.0.1", 9991);
        }

        public static event Action<TimeKindUpdatedRequestMessage> TimeKindUpdated;
    }
}
