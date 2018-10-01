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
            if (options.DoBuild)
                BuildPackageCatalogue(prime, options.PubConfigPath);

            if (options.DoPublish)
                CatalogueBootEntry.Publish(prime, options.PubConfigPath);
        }

        public static ContentUri BuildPackageCatalogue(PrimeInstance prime, string configPath)
        {
            var config = CatalogueBootEntry.GetPublisherConfig(prime, configPath);
            if (config == null)
                return new ContentUri();

            var packageBuilder = new PackageCatalogueBuilder(prime, config);
            return packageBuilder.Build();
        }
    }
}
