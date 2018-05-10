using System;
using System.Collections.Generic;
using Prime.Core.Encryption;
using Prime.Core;
using Prime.Extensions;
using Prime.PackageManager;

namespace Prime.ConsoleApp.Tests.Frank
{
    public class Frank
    {
        public static void Go(ServerContext context)
        {
            AuthManagerTest.EcdsaKeySign(context);

            //new ExtensionLoader().Compose();

            PackageCoordinator(context);

            PackageCatalogue(context);
        }

        private static void PackageCoordinator(ServerContext context)
        {
            var pm = new PackageCoordinator(context);

            pm.EnsureInstalled();

            context.Logger.Info(pm.Distribution.Count);
        }

        private static void PackageCatalogue(ServerContext context)
        {
            var cbuild = new CatalogueBuilder(context);
            cbuild.Build();
        }
    }
}
