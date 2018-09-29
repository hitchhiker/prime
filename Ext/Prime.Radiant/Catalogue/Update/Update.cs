using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.Radiant.Catalogue
{
    public class Update : CommonBase
    {
        private readonly PrimeInstance _instance;
        public Update(PrimeInstance instance) : base(instance.C)
        {
            _instance = instance;
        }

        public bool UpdateAll()
        {
            foreach (var e in C.Config.CatalogueConfig.Subscribed.Entries)
                UpdateCatalogue(ContentUri.Parse(e.UriString));
            
            return true;
        }

        public bool UpdateCatalogue(ContentUri uri)
        {
            L.Info($"Attempting to resolve {uri}");

            var indexNode = M.SendAndWait<GetNsResolveRequest, GetNsResolveResponse>(new GetNsResolveRequest(uri.Path));
            if (indexNode == null || !indexNode.Success)
            {
                L.Fatal("Could not resolve IPNS location. Aborting.");
                return false;
            }

            L.Info("Catalogue index found @ " + indexNode.ContentUri);

            var tmpDir = C.FileSystem.GetTmpSubDirectory("cat-download", "tmp-" + ObjectId.NewObjectId());

            var pc = new PublishCatalogue(_instance);

            var index = pc.DownloadIndex(tmpDir, indexNode.ContentUri);
            if (index == null)
                return false;

            var catalogue = pc.DownloadCatalogue(tmpDir, index);
            if (catalogue == null)
                return false;

            return catalogue.DoInstall(_instance);
        }
    }
}
