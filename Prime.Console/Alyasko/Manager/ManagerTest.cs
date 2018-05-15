using System.Net.Sockets;
using Prime.Console.Frank;
using Prime.Core;
using Prime.KeysManager;
using Prime.Manager;
using Prime.WebSocketServer;

namespace Prime.Console.Alyasko.Manager
{
    public class ManagerTest : TestClientServerBase
    {
        public ManagerTest(ServerContext server, ClientContext c) : base(server, c)
        {
        }

        public override void Go()
        {
            var server = new MessagingServer.Server(S);
            server.Inject(new ServerExtension());

            var managerExt = new ManagerServiceExtension();
            managerExt.Main(S);
            
            server.Start();
        }
    }
}