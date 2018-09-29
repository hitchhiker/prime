using System.IO;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;
using Prime.Radiant.Catalogue;

namespace Prime.Radiant
{
    public static class CatalogueBootEntry
    {
        public static void Update(PrimeInstance prime)
        {
            var u = new Update(prime);
            u.UpdateAll();
        }

        public static void Publish(PrimeInstance prime, PrimeBootOptions.Publish options)
        {
            Publish(prime, options.PubConfigPath);
        }

        public static void Publish(PrimeInstance prime, string pubConfigPath)
        {
            pubConfigPath = pubConfigPath.ResolveSpecial();

            if (!File.Exists(pubConfigPath))
            {
                prime.L.Error(pubConfigPath + " does not exist. Aborting.");
                return;
            }

            var txt = File.ReadAllText(pubConfigPath);
            var config = JsonConvert.DeserializeObject<CataloguePublisherConfig>(txt);

            if (config == null)
            {
                prime.L.Error(pubConfigPath + " isn't a valid configuration file. Aborting.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(config.HashSource))
            {
                PublishCatalogue(prime, config, new ContentUri() {Path = config.HashSource, Protocol = "ipfs"});
                return;
            }

            if (!string.IsNullOrWhiteSpace(config.IpnsSource))
            {
                PublishCatalogueViaNs(prime, config);
                return;
            }
        }

        private static void PublishCatalogueViaNs(PrimeInstance prime, CataloguePublisherConfig config)
        { 
            prime.L.Info($"Attempting to resolve ipns://{config.IpnsSource}");

            var indexNode = prime.M.SendAndWait<GetNsResolveRequest, GetNsResolveResponse>(new GetNsResolveRequest(config.IpnsSource));
            if (indexNode == null || !indexNode.Success)
            {
                prime.L.Fatal("Could not resolve IPNS location. Aborting.");
                return;
            }

            PublishCatalogue(prime, config, indexNode.ContentUri);
        }

        private static void PublishCatalogue(PrimeInstance prime, CataloguePublisherConfig config, ContentUri indexUri)
        {
            prime.Start();
            var publisher = new PublishCatalogue(prime);
            publisher.Publish(config, indexUri);
        }
    }
}