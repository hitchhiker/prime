using Prime.Base.DStore;
using Prime.Core;

namespace Prime.Scratch
{
    public class FrankTest
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

            //var indexUri = PackageTests.PackageCatalogue(client);
            //PackageTests.PublishCatalogue(client, indexUri);

            PackageTests.PublishCatalogue(client, new ContentUri {Path= "QmRsMdjfNDi763H3LjUPot522dncz7iBGmk5xevSjJDKh1", Protocol = "ipfs"});

            //PackageTests.PublishCatalogue(client, PackageTests.GetCataloguePublisherConfig());
        }
    }
}
