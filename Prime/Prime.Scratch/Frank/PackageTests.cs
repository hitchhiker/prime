using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Prime.Base.DStore;
using Prime.Core;
using Prime.PackageManager;
using Prime.Radiant;
using Prime.Radiant.Catalogue;

namespace Prime.Scratch
{
    public static class PackageTests
    {
        public static void Go(PrimeInstance prime)
        {
            //var opt = new PrimeBootOptions.Packages() {PubConfigPath = "[src]/instance/prime_package_catalogue.config" };

            prime.Start();
            //CatalogueBootEntry.Publish(prime, opt.PubConfigPath);

            var cat = prime.C.Config.CatalogueConfig.Subscribed.Entries.FirstOrDefault();
            var u = new Update(prime);
            u.UpdateCatalogue(cat.GetSelf());

            //PackageCatalogueEntry.BuildPackageCatalogue(prime, opt.PubConfigPath);
            //PackageTests.PackageCoordinator(s);

            //var indexUri = PackageTests.PackageCatalogue(client);
            //PackageTests.PublishCatalogue(client, indexUri);
            //PackageTests.PublishCatalogue(client, new ContentUri {Path= "QmRsMdjfNDi763H3LjUPot522dncz7iBGmk5xevSjJDKh1", Protocol = "ipfs"});

            //client.Start();
            //CatalogueBootEntry.Publish(client, "[src]/instance/prime_main_catalogue.config");

            //PackageTests.PublishCatalogue(client, PackageTests.GetCataloguePublisherConfig());

            //client.Start();
            //var u = new Update(client);
            //u.UpdateCatalogue(ContentUri.Parse("ipns://QmdGeEiLVqo5nu9U9f5y6ckV3RX3hZ5fDMnMHZSKfm1NP7"));
        }

        public static void PackageUpdate(PrimeInstance prime)
        {/*
            prime.Start();

            PackageCatalogueEntry.Request(prime,
                new PrimeBootOptions.Packages()
                {
                    DoBuild = true,
                    PubConfigPath = Path.Combine(prime.C.Config.ConfigLoadedFrom.Directory.FullName, "prime_package_catalogue.config")
                });*/
        }

        public static void PackageCoordinator(PrimeContext context)
        {
            //var pm = new PackageCoordinator(context);

            //pm.EnsureInstalled();

            //context.L.Info(pm.Distribution.Count);
        }

        public static ContentUri PackageCatalogue(PrimeInstance prime, CataloguePublisherConfig pubConfig)
        {
            prime.Start();

            var packageBuilder = new PackageCatalogueEntry(prime, pubConfig);

            return packageBuilder.Build();
        }

        public static void PublishCatalogue(PrimeInstance prime, CataloguePublisherConfig config)
        {
            prime.Start();
            var catDir = config.GetCatalogueDirectory(prime.C);

            var fi = new FileInfo(Path.Combine(catDir.FullName, CatalogueHelper.IndexName));
            if (!fi.Exists)
            {
                prime.C.L.Fatal("No index file found for catalogue: " + config.CatalogueName);
                return;
            }

            var indexNode = prime.C.M.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(fi.FullName));
            if (indexNode == null || !indexNode.Success)
            {
                prime.C.L.Fatal("Index file could not be added to IPFS: " + config.CatalogueName);
                return;
            }

            PublishCatalogue(prime, indexNode.ContentUri, config);
        }

        public static void PublishCatalogue(PrimeInstance prime, ContentUri indexUri, CataloguePublisherConfig pubConfig)
        {
            prime.Start();

            var path = Path.Combine(prime.C.FileSystem.TmpDirectory.FullName, "pub.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(pubConfig, Formatting.Indented));

            var publisher = new PublishCatalogue(prime);
            publisher.Publish(pubConfig, indexUri);
        }
    }
}