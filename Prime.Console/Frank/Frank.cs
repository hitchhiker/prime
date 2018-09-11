using Prime.Console.Frank.Ipfs;
using Prime.Console.Frank.Socket;
using Prime.Console.Windows.Alyasko.WebSocket;
using Prime.Core;

namespace Prime.Console.Frank
{
    public class Frank
    {
        public static void Go(Core.PrimeInstance primeInstance, ClientContext c)
        {
            var s = primeInstance.Context;
            //AuthManagerTest.EcdsaKeySign(s);
            AuthManagerTest.Key1(s);
            //new IpfsMessageTest(primeInstance).Go();

            //new ReflectionTests2(primeInstance).Go();

            //new ReflectionTests1(s).Go();

            //new WebSocketServerTest(s, c).Go();

            //new SocketServerTest(s, c).Go();

            //

            //new ExtensionLoader().Compose();

            //PackageTests.PackageCoordinator(s);

            //PackageTests.PackageCatalogue(c);
        }
    }
}
