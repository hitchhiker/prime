using Prime.Base.DStore;
using Prime.Console.Frank.Ipfs;
using Prime.Console.Frank.Socket;
using Prime.Console.Windows.Alyasko.WebSocket;
using Prime.Core;

namespace Prime.Console.Frank
{
    public class Frank
    {
        public static void Go(PrimeInstance server, PrimeInstance client)
        {
            var s = server.C;
            var c = client.C;

            //AuthManagerTest.EcdsaKeySign(s);
            //AuthManagerTest.Key1(s);

            //new IpfsMessageTest(primeInstance).Go();

            //new ReflectionTests2(primeInstance).Go();

            //new ReflectionTests1(s).Go();

            //new WebSocketServerTest(s, c).Go();

            //new SocketServerTest(s, c).Go();

            //

            //new ExtensionLoader().Compose();

            //PackageTests.PackageCoordinator(s);

            //PackageTests.PackageCatalogue(client);

            PackageTests.PublishCatalogue(client, new ContentUri {Path= "QmRsMdjfNDi763H3LjUPot522dncz7iBGmk5xevSjJDKh1", Protocol = "ipfs"});
        }
    }
}
