using System.IO;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;
using Prime.Radiant.Catalogue;

namespace Prime.Radiant
{
    public static class RadiantEntry
    {
        public static void EnterUpdate(PrimeInstance prime, RadiantArguments.UpdateArguments args)
        {
            if (args.DoAll)
                UpdateAll(prime);
            else if (args.CataloguePubKey != null)
                Update(prime, args.CataloguePubKey);
        }

        public static void EnterPublish(PrimeInstance prime, RadiantArguments.PublishArguments args)
        {
            Publish(prime, args.PubConfigPath);
        }

        public static void Update(PrimeInstance prime, string publicKey)
        {
            var u = new Update(prime);
            u.UpdateCatalogue(publicKey);
        }

        public static void UpdateAll(PrimeInstance prime)
        {
            var u = new Update(prime);
            u.UpdateAll();
        }

        public static void Publish(PrimeInstance prime, string pubConfigPath)
        {
            var config = CataloguePublisherConfig.Get(prime, pubConfigPath);
            if (config == null)
                return;
            
            if (config.IsLocalSource)
                PublishCatalogueLocal(prime, config);
            else if (!string.IsNullOrWhiteSpace(config.HashSource))
                PublishCatalogue(prime, config, new ContentUri() {Path = config.HashSource, Protocol = "ipfs"});
            else if (!string.IsNullOrWhiteSpace(config.IpnsSource))
                PublishCatalogueViaNs(prime, config);
        }

        private static void PublishCatalogueLocal(PrimeInstance prime, CataloguePublisherConfig config)
        {
            var indexFi = config.GetIndexArchiveInfo(prime.C);
            var index = CatalogueHelper.ExtractIndex(prime.C, indexFi);

            if (index == null)
            {
                prime.C.L.Fatal("Unable to extract the index archive for catalogue: " + config.CatalogueName);
                return;
            }

            var indexNode = prime.C.M.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(indexFi.FullName));
            if (indexNode == null || !indexNode.Success)
            {
                prime.C.L.Fatal("Index archive could not be added to IPFS: " + config.CatalogueName);
                return;
            }

            PublishCatalogue(prime, config, indexNode.ContentUri);
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