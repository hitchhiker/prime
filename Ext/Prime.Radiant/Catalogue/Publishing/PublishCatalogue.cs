using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;
using Prime.Radiant.Catalogue;

namespace Prime.Radiant
{
    public class PublishCatalogue : CommonBase
    {
        private readonly PrimeInstance _prime;

        public PublishCatalogue(PrimeInstance prime) : base(prime.C)
        {
            _prime = prime;
        }

        public void Publish(CataloguePublisherConfig config, ContentUri indexUri)
        {
            var tmpDir = C.FileSystem.GetTmpSubDirectory("cat-download", "tmp-" + ObjectId.NewObjectId());

            Publish(config, indexUri, tmpDir);

            tmpDir.Delete(true);
        }

        private void Publish(CataloguePublisherConfig config, ContentUri indexUri, DirectoryInfo tmpDir)
        {
            //download catalogue index, deserialise

            var index = DownloadIndex(tmpDir, indexUri);
            if (index == null)
                return;

            //download latest catalogue

            var catalogue = DownloadCatalogue(tmpDir, index);
            if (catalogue == null)
                return;

            //pin all entries

            PinCatalogue(catalogue);

            //publish ipns

            PublishNs(config, indexUri);
        }

        public CatalogueIndex DownloadIndex(DirectoryInfo tmpDir, ContentUri indexUri) {

            var indexPath = Path.Combine(tmpDir.FullName, "index.json");

            L.Info($"Attempting downloaded of catalogue index from: {indexUri}..");

            var response = M.SendAndWait<GetContentRequest, GetContentResponse>(new GetContentRequest(indexPath, indexUri.Path));
            if (response == null || !response.Success)
            {
                L.Fatal("Unable to get the catalogue index file from IPFS. Aborting.");
                return null;
            }

            if (!File.Exists(indexPath))
            {
                L.Fatal("Apparently retreived the catalogue index file from IPFS, but nothing appeared locally. Aborting.");
                return null;
            }

            var index = JsonConvert.DeserializeObject<CatalogueIndex>(File.ReadAllText(indexPath));
            if (index == null)
            {
                L.Fatal("Apparently retreived the catalogue index file from IPFS, but nothing appeared locally. Aborting.");
                return null;
            }

            L.Info($"Catalogue index downloaded. Revision #{index.CurrentRevision}");

            return index;
        }

        public ICatalogue DownloadCatalogue(DirectoryInfo tmpDir, CatalogueIndex index)
        {
            var type = Helper.GetCatalogueType(_prime, index.CatalogueType);
            if (type == null)
            {
                L.Fatal($"Unable to find the catalogue type '{index.CatalogueType}', this may be due to a missing extension. Aborting.");
                return null;
            }

            var latest = index.GetLatest();
          
            var catPath = Path.Combine(tmpDir.FullName, "cat.json");

            L.Info($"Attempting downloaded of catalogue from: {latest.Uri}..");

            var response = M.SendAndWait<GetContentRequest, GetContentResponse>(new GetContentRequest(catPath, latest.Uri.Path));
            if (response == null || !response.Success)
            {
                L.Fatal("Unable to get the catalogue file from IPFS. Aborting.");
                return null;
            }

            if (!File.Exists(catPath))
            {
                L.Fatal("Apparently retreived the catalogue file from IPFS, but nothing appeared locally. Aborting.");
                return null;
            }

            if (!(JsonConvert.DeserializeObject(File.ReadAllText(catPath), type) is ICatalogue catalogue))
            {
                L.Fatal("Retreived the catalogue file from IPFS, but was unable to deserialise it. Aborting.");
                return null;
            }

            L.Info($"'{catalogue.CatalogueTypeName}' catalogue downloaded: '{catalogue.Name}'");

            return catalogue;
        }

        private void PinCatalogue(ICatalogue catalogue)
        {
            var content = catalogue.AllContentUri();

            L.Info($"Pinning {content.Count} entries from catalogue..");

            foreach (var i in content)
            {
                L.Info("Pinning: " + i);
                M.SendAndWait<PinContentRequest, PinContentResponse>(new PinContentRequest(i));
            }

            L.Info("Catalogue pinned.");
        }

        private void PublishNs(CataloguePublisherConfig config, ContentUri indexUri)
        {
            L.Info($"Publishing name record: {config.IpnsKeyPublic} key:\'{config.IpnsKeyName}\' > {indexUri} ..");

            M.SendAndWait<PublishNsRequest, PublishNsResponse>(new PublishNsRequest(indexUri.Path, config.IpnsKeyName));

            L.Info("Catalogue published.");
        }
    }
}
