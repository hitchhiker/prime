using Prime.Console.Alyasko.Manager;
using Prime.Console.Windows.Alyasko.WebSocket;
using Prime.Core;

namespace Prime.Console.Windows.Alyasko
{
    public static class AlyaskoTest
    {
        public static void Go(ServerContext serverCtx, ClientContext clientCtx)
        {
            var logger = new ConsoleLogger();
            serverCtx.L = logger;
            logger.Log("Alyasko Test started...");

            //new WebSocketServerTest(serverCtx, clientCtx).Go();
            new ManagerTest(serverCtx, clientCtx).Go();
        }
    }
}
