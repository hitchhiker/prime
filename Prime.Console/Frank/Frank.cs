using System;
using System.Collections.Generic;
using Prime.Core.Encryption;
using Prime.Core;
using Prime.Extensions;

namespace Prime.ConsoleApp.Tests.Frank
{
    public class Frank
    {
        public static void Go(ServerContext server, ClientContext client)
        {
            new SocketServerTest().Go(server, client);

            //AuthManagerTest.EcdsaKeySign(client);

            //new ExtensionLoader().Compose();

            //PackageTests.PackageCoordinator(server);

            //PackageTests.PackageCatalogue(client);
        }
    }
}
