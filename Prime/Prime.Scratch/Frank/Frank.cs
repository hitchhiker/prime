using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Prime.Base;
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

            //AuthManagerTest.Key1(c);

            //new Prime.NetCoreExtensionPackager.ReflectionTest(c).Go();
            //new ReflectionTests3(client).Go();

            //CommandArgsTester.Go(client, "package publish --pubconfig [src]/instance/prime_package_catalogue.config  -p [src]/instance/prime_package_compile.config");

            //CommandArgsTester.Go(client, "package compile -p [src]/instance/prime_package_compile.config");

            //CommandArgsTester.Go(client, "package bundle -p [src]/instance/prime_package_compile.config");

            //CommandArgsTester.Go(client, "package build -p [src]/instance/prime_package_catalogue.config");

            //CommandArgsTester.Go(client, "package upgrade");

            //CommandArgsTester.Go(client, "catalogue publish -p [src]/instance/prime_package_catalogue.config");

            //CommandArgsTester.Go(client, "catalogue update --all");

            //CommandArgsTester.Go(client, "--help");

            //CommandArgsTester.Go(client, "catalogue");

            //CommandArgsTester.Go(client, "daemon");

            CommandArgsTester.Go(client, "ipfs --././upgr   daemon");

            //PackageTests.Go(client);

            //var config = Radiant.PackageConfig.Get(c, "D:\\hh\\git\\prime\\src\\instance\\prime_package_compile.config", false);
            //config.Save();
        }
    }
}
