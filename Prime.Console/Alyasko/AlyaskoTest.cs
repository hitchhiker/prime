using Prime.Console.Windows.Alyasko.WebSocket;
using Prime.Core;

namespace Prime.Console.Windows.Alyasko
{
    public static class AlyaskoTest
    {
        public static void Go(ServerContext serverCtx, ClientContext clientCtx)
        {
            var logger = serverCtx.L;

            logger.Log("Alyasko Test started...");

            new WebSocketServerTest(serverCtx, clientCtx).Go();
        }
    }
}
