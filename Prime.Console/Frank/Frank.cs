using Prime.Console.Windows.Alyasko.WebSocket;
using Prime.Core;

namespace Prime.Console.Frank
{
    public class Frank
    {
        public static void Go(ServerContext s, ClientContext c)
        {
            //new ReflectionTests(s).Go();

            //new IpfsMessageTest(s).Go();

            new WebSocketServerTest(s, c).Go();

            //new SocketServerTest(s, c).Go();

            //AuthManagerTest.EcdsaKeySign(s);

            //new ExtensionLoader().Compose();

            //PackageTests.PackageCoordinator(s);

            //PackageTests.PackageCatalogue(s);
        }
    }
}
