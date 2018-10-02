using Prime.Base.DStore;
using Prime.Base.Encryption;
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

            //AuthManagerTest.Key1(c);

            CommandArgsTester.Go(client, "package compile -p [src]/instance/prime_package.config");

            //CommandArgsTester.Go(client, "package build -p [src]/instance/prime_package_catalogue.config");

            //CommandArgsTester.Go(client, "catalogue publish -p [src]/instance/prime_package_catalogue.config");
            //
            
            //CommandArgsTester.Go(client, "--help");

            //CommandArgsTester.Go(client, "catalogue");

            //PackageTests.Go(client);
        }
    }
}
