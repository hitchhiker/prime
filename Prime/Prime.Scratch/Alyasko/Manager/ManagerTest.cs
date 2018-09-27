using System.Net.Sockets;
using Prime.Scratch.Frank;
using Prime.Core;
using Prime.Finance.Services.Services.Binance;
using Prime.KeysManager;
using Prime.Manager;
using Prime.WebSocketServer;

namespace Prime.Scratch.Alyasko.Manager
{
    public class ManagerTest : TestClientServerBase
    {
        public ManagerTest(PrimeContext serverContext, PrimeContext c) : base(serverContext, c)
        {
        }

        public override void Go()
        {
            var server = new MessagingServer.Server(S);
            //server.Inject(new WebSocketServer.ServerExtension());

            var managerExt = new ManagerServiceExtension();
            managerExt.Main(S);
            
            server.Start();
        }
    }
}