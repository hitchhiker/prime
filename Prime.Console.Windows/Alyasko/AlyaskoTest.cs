using System;
using System.Collections.Generic;
using System.Text;
using Prime.Console.Alyasko.WebSocket;
using Prime.Core;

namespace Prime.Console.Alyasko
{
    public static class AlyaskoTest
    {
        public static void Go(ServerContext serverCtx, ClientContext clientCtx)
        {
            var logger = serverCtx.L;

            logger.Log("Alyasko Test started...");

            var test = new WebSocketServerTest(serverCtx, clientCtx);
            test.Run();
        }
    }
}
