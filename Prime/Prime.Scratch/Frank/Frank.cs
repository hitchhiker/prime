using Prime.Base.DStore;
using Prime.Core;
using Prime.Radiant;
using Prime.Radiant.Catalogue;

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

            //PackageTests.PublishCatalogue(client, new ContentUri {Path= "QmRsMdjfNDi763H3LjUPot522dncz7iBGmk5xevSjJDKh1", Protocol = "ipfs"});

            //client.Start();
            //CatalogueBootEntry.Publish(client, "[src]/instance/prime_main_catalogue.config");

            //PackageTests.PublishCatalogue(client, PackageTests.GetCataloguePublisherConfig());

            client.Start();
            var u = new Update(client);
            u.UpdateCatalogue(ContentUri.Parse("ipns://QmdGeEiLVqo5nu9U9f5y6ckV3RX3hZ5fDMnMHZSKfm1NP7"));
        }
    }
}
