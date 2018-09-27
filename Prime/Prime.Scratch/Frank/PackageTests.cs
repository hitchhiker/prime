using System.IO;
using Newtonsoft.Json;
using Prime.Base.DStore;
using Prime.Core;
using Prime.PackageManager;
using Prime.Radiant;

namespace Prime.Scratch
{
    public static class PackageTests
    {
        public static void PackageCoordinator(PrimeContext context)
        {
            //var pm = new PackageCoordinator(context);

            //pm.EnsureInstalled();

            //context.L.Info(pm.Distribution.Count);
        }

        public static CataloguePublisherConfig GetCataloguePublisherConfig()
        {
            var pubConfig = new CataloguePublisherConfig()
            {
                CatalogueName = "prime_main_catalogue",
                IpnsKeyPublic = "QmbvBhusH3RLGywcpsovagYwEBN9EFegqJg6v9kvk48gmx", //local/hh10
                IpnsKeyName = "prime_main_catalogue",
                PriKey = "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",
                PubKey = "mQENBFuXmnUDCACNp0oG0IJbrVPvhP+TV4vhLsPQQYh4Oauz9gWEmOPu/4AZG2TJUS0lyVHbupXWlbgiss/QPCG0+8F6Y1UaU+wYvdS8NxokmqmU+M2tSlcP1aeHhsr36TWXaUykR6IIXb0rlgAOFC2i1aIt0x6AWVW7Rp33XwbarniYey9ErcEOzINfKOLO+OuV+8fzpmd0G9m46680OojSCc6cLe5/lxkcctIkT6LmI/tHFTbgvANHP3Zpu0QxSGEADxF5++UU+zXUFnJHqigjgtED6RzJgLeN5JLgG21oKui4xVLPom/dQSnnGmR9VgMqllgZILnISqIzpaLHTXDBMJe49yqNAx1NABEBAAG0CnByaW1lLXVzZXKJATcEEAMCACEFAluXmnUCGwMECwkIBwYVCAIJCgsECwkIBwYVCAIJCgsACgkQi+1ZjdqvVooccwf/WIUmDNPQN/4gJ4ci2JP0vU4JbZURE9ATFGKoAMK3mpSTwPVXrR08ZMIk7Vp80F+mYAPoJYVOjMMSCMvA4ey3oqnLVqwn4SETj0xDCOeYHMoapwCDsy+WoyFYh0T4ChDtFDRLNAgEaE1le3KT+hfhZL/q6D5u3dNLXVTRulTKNIZ2YXMyx6psgGCJkLd1DzHsjA17sm19tCnHntexRtgTJzTnuozNo9yBRe0UB3MD61Rjr9/ugwCXLNQKOelEfNmcjpS5+gYW5BWTkHF8poRTB+bJESVhlJdMVExD4TOIYFcsW7Jp4zn5k8f6czfLk5QKJbl7+LKNdcbIZ5YO3KKedrkBDQRbl5p1AQgAoLEt5gr2W6SYy4ib1mxNDJ+y0JoRSCcfgNDSKuGGbKu/t4JZMyS3tA/EZyWbxDW4esxAR+Ttui80Q7a9k/K9TfsXhwc7IRohUAzxDnWU4huzU7LOiT2XYo5Uh6BuTa7Gm5RRANtir7V8gR6kK+dmVIRi90v5SUvcw6FRKE+uHNyKDYGWt5itCsdCtFUdM3jA+9aqV40d+g3ERxq3IkZr6JLTqUgaXoy6/ANQ6sWktKA/+eRJvB8QklA4AwClQUDPKk/6r8TQ1fVTNhviQQ+5rIdM3/74BZtuBRw+dQbRYbMxeFFRkP4FP7Aq2oKFLidp5gTiCcHWi9dO4hTdXSV8rwARAQABiQEfBBgDAgAJBQJbl5p2AhsMAAoJEIvtWY3ar1aKQsIH/1ZKqQKNdksxnKEamFA/yZipqfTbc85ZzJv056Or19BBesbOGPryHvUTMuL3wZ788/Fy3cvOkFf5WAFl9tZ0mzzeFZPiTbIEH3SCQDl6K51hvrbQBcvO0FCe/Bei7IH2ETkmyK2gWcHdWyC7gFpCnKIw29jvHdH1VU2cGQFqUsNsUxyPQGCapBCwitmdHeRytI79cnQTY010Q/HXDR9Wit6V7dHqHzjfKlUsWpX/ri33nRrWbFWMNcTvKgNQOhYhPf6hQBZCd2iis+/bPKs9wmqbK0xION9s/V3PsZvJnCAjS4c/LcEmRcRELWpHkrv0M+fz/epgEq7OefBB9Su61+A="
            };
            
            return pubConfig;
        }

        public static ContentUri PackageCatalogue(PrimeInstance prime)
        {
            prime.Start();

            var pubConfig = GetCataloguePublisherConfig();

            var packageBuilder = new PackageCatalogueBuilder(prime, pubConfig);

            return packageBuilder.Build();
        }

        public static void PublishCatalogue(PrimeInstance prime, CataloguePublisherConfig config)
        {
            prime.Start();
            var catDir = config.GetCatalogueDirectory(prime.C);

            var fi = new FileInfo(Path.Combine(catDir.FullName, CataloguePublisherConfig.IndexName));
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

            PublishCatalogue(prime, indexNode.ContentUri);
        }

        public static void PublishCatalogue(PrimeInstance prime, ContentUri indexUri)
        {
            prime.Start();

            var pubConfig = GetCataloguePublisherConfig();

            var path = Path.Combine(prime.C.FileSystem.TmpDirectory.FullName, "pub.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(pubConfig, Formatting.Indented));

            var publisher = new PublishCatalogue(prime);
            publisher.Publish(pubConfig, indexUri);
        }
    }
}