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
        public ContentUri LastIndexUri { get; private set; }

        public CatalogueBuilder(PrimeInstance prime, ICatalogueBuilder builder, CataloguePublisherConfig config) : base(prime.C)
        {
            _builder = builder;
            _config = config;
        }

        public ContentUri Build()
        {
            var cDir = C.FileSystem.CatalogueDirectory;

            var catalogue = _builder.CompileCatalogue();

            if (catalogue == null)
                return new ContentUri();

            catalogue.Name = _config.CatalogueName;

            if (!WriteContent(catalogue))
                return new ContentUri();

            return LastIndexUri;
        }

        private bool WriteContent(ICatalogue catalogue)
        {
            var catDir = _config.GetCatalogueDirectory(C);
            var buildDir = C.FileSystem.GetTmpSubDirectory("cat-build");

            var tmpCatFi = buildDir.GetFile($"cat-{ObjectId.NewObjectId()}.json");

            File.WriteAllText(tmpCatFi.FullName, JsonConvert.SerializeObject(catalogue, Formatting.Indented));

            L.Info("Catalogue compilation complete, now determining status...");

            var response = M.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(tmpCatFi.FullName));
            if (response == null || !response.Success)
            {
                L.Fatal("Unable to add the catalogue file to IPFS. Aborting.");
                return false;
            }

            var justCatHash = response.ContentUri.Path;
            var archiveName = $"cat-{justCatHash}.arc";

            var finalArcFi = catDir.GetFile(archiveName);

            if (finalArcFi.Exists)
            {
                tmpCatFi.Delete();
                L.Log("");
                L.Info("No changes detected to catalogue, finished.");
                L.Log("Catalogue HASH: " + justCatHash);
                return false;
            }

            //sign and archive

            var signatureFi = tmpCatFi.CreateSignedFile(_config.PriKey.ToPrivateKey());
            var tmpCatArcFi = buildDir.CreateArchive(tmpCatFi, signatureFi, archiveName);

            signatureFi.Delete();

            var arcHashResponse = M.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(tmpCatArcFi.FullName));

            if (arcHashResponse == null || !arcHashResponse.Success)
            {
                L.Fatal("Unable to add the archive file to IPFS. Aborting.");
                return false;
            }

            var arcHash = arcHashResponse.ContentUri;

            File.Move(tmpCatArcFi.FullName, finalArcFi.FullName);
            
            //create and sign index

            var indexUri = CreateIndexArchive(catDir, arcHash);

            if (indexUri.Path==null)
                return false;

            var finalNode = M.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(catDir.FullName));
            if (finalNode == null || !finalNode.Success)
            {
                L.Fatal("Unable to add the catalogue directory to IPFS. Aborting.");
                return false;
            }

            L.Log("");
            L.Log("Catalogue Index HASH: " + indexUri.Path);
            L.Log("Catalogue Archive HASH: " + arcHash);
            L.Log("Catalogue HASH: " + justCatHash);

            catalogue.BuildInformation = new CatalogueBuildInformation() {IndexUri = indexUri};

            return true;
        }

        private ContentUri CreateIndexArchive(DirectoryInfo catDir, ContentUri catArchiveUri)
        {
            var indexFi = catDir.GetFile(CatalogueHelper.IndexName);
            var index = CatalogueHelper.ExtractIndexFromCatalogueDirectory(C, catDir);

            L.Log($"Previous index was at revision #{index.CurrentRevision}.");

            index.CatalogueType = _builder.TypeName; 
            index.Name = _config.CatalogueName;
            index.CurrentRevision++;
            index.Entries.Add(new CatalogueIndexEntry {Revision =  index.CurrentRevision, ArchiveUri = catArchiveUri, UtcCreated = DateTime.UtcNow});

            var indexContent = JsonConvert.SerializeObject(index, Formatting.Indented);

            File.WriteAllText(indexFi.FullName, indexContent);
            indexFi.Refresh();
            var signatureFi = indexFi.CreateSignedFile(_config.PriKey);
            var indexArcFi = catDir.CreateArchive(indexFi, signatureFi, "index.arc");

            signatureFi.Delete();
            indexFi.Delete();

            var indexArcUri = M.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(indexArcFi.FullName));
            if (indexArcUri?.Success != true)
            {
                L.Fatal("Unable to add the catalogue index file to IPFS. Aborting.");
                return new ContentUri();
            }

            return LastIndexUri = indexArcUri.ContentUri;
        }
    }
}