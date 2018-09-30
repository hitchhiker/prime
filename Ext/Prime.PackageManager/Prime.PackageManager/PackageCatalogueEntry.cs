using System;
using System.Collections.Generic;
using System.Text;
using Prime.Base.DStore;
using Prime.Base.Messaging.Common;
using Prime.Core;
using Prime.Radiant;

namespace Prime.PackageManager
{
    public class PackageCatalogueEntry
    {
        public static void Request(PrimeInstance prime, PrimeBootOptions.Packages options)
        {
            Console.WriteLine("publisher config: " + options.PubConfigPath);

            if (options.DoBuild)
                PackageCatalogue(prime, options);
        }

        public static ContentUri PackageCatalogue(PrimeInstance prime, PrimeBootOptions.Packages options)
        {
            var pubConfig = CatalogueBootEntry.GetPublisherConfig(prime, options.PubConfigPath);
            if (pubConfig == null)
                return new ContentUri();

            var packageBuilder = new PackageCatalogueBuilder(prime, pubConfig);
            return packageBuilder.Build();
        }
    }
}
