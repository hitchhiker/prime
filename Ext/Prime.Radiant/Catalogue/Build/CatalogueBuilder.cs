using System;
using System.IO;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.Radiant
{
    public class CatalogueBuilder : CommonBase
    {
        private readonly ICatalogueBuilder _builder;
        private readonly CataloguePublisherConfig _config;

        public CatalogueBuilder(PrimeInstance prime, ICatalogueBuilder builder, CataloguePublisherConfig config) : base(prime.C)
        {
            _builder = builder;
            _config = config;
        }

        public ICatalogue Build()
        {
            var cDir = C.FileSystem.CatalogueDirectory;

            var catalogue = _builder.CompileCatalogue();

            if (catalogue == null)
                return null;

            catalogue.Name = _config.CatalogueName;

            if (!WriteOut(cDir, catalogue))
                return null;

            return catalogue;
        }

        private bool WriteOut(DirectoryInfo rootDir, ICatalogue catalogue)
        {
            var catDir = rootDir.CreateSubdirectory($"cat-{_config.CatalogueId}-{_config.CatalogueName}");

            var tmpFi = new FileInfo(Path.Combine(C.FileSystem.GetTmpSubDirectory("cat-build").FullName, $"cat-{ObjectId.NewObjectId()}.json"));

            File.WriteAllText(tmpFi.FullName, JsonConvert.SerializeObject(catalogue, Formatting.Indented));

            L.Info("Catalogue compilation complete, now determining status...");

            var response = M.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(tmpFi.FullName));
            if (response == null || !response.Success)
            {
                L.Fatal("Unable to add the catalogue file to IPFS. Aborting.");
                return false;
            }

            var hash = response.ContentUri.Path;

            var finalFi = new FileInfo(Path.Combine(catDir.FullName, $"cat-{hash}.json"));

            if (finalFi.Exists)
            {
                tmpFi.Delete();
                L.Info("No changes detected to catalogue, finished.");
                return false;
            }

            File.Move(tmpFi.FullName, finalFi.FullName);

            var indexUri = CreateIndex(catDir, response.ContentUri);

            if (indexUri.Path==null)
                return false;

            var finalNode = M.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(catDir.FullName));
            if (finalNode == null || !finalNode.Success)
            {
                L.Fatal("Unable to add the catalogue directory to IPFS. Aborting.");
                return false;
            }

            L.Log("Catalogue IPFS HASH: " + indexUri.Path);
            L.Log("Catalogue Location: " + catDir.FullName);

            catalogue.BuildInformation = new CatalogueBuildInformation() {IndexUri = indexUri};

            return true;
        }

        private ContentUri CreateIndex(DirectoryInfo catDir, ContentUri latestUri)
        {
            var indexFi = new FileInfo(Path.Combine(catDir.FullName, "index.json"));

            var index = !indexFi.Exists ? 
                new CatalogueIndex() {CurrentRevision = 0, UtcCreated = DateTime.UtcNow} : 
                JsonConvert.DeserializeObject<CatalogueIndex>(File.ReadAllText(indexFi.FullName));

            index.CatalogueType = _builder.TypeName;
            index.Name = _config.CatalogueName;
            index.CurrentRevision++;
            index.Entries.Add(new CatalogueIndexEntry {Revision =  index.CurrentRevision, Uri = latestUri, UtcCreated = DateTime.UtcNow});

            var indexContent = JsonConvert.SerializeObject(index, Formatting.Indented);

            File.WriteAllText(indexFi.FullName, indexContent);

            var indexUri = M.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(indexFi.FullName));
            if (indexUri?.Success == true)
                return indexUri.ContentUri;

            L.Fatal("Unable to add the catalogue index file to IPFS. Aborting.");
            return new ContentUri();
        }
    }
}