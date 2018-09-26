using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.PackageManager
{
    public class CatalogueBuilder : CommonBase
    {
        public CatalogueBuilder(PrimeInstance prime) : base(prime.C)
        {
        }

        public Catalogue Build(CataloguePublisherConfig config)
        {
            var dDir = C.FileSystem.DistributionDirectory;
            var cDir = C.FileSystem.CatalogueDirectory;

            var dirs = dDir.EnumerateDirectories().ToList();

            L.Info("Scanning distribution directory: " + dDir.FullName);

            if (dirs.Count == 0)
            {
                L.Warn("Distribution directory is empty.");
                return null;
            }

            var cat = Discover(dirs);

            L.Info($"{cat.Count} entries found, compiling catalogue..");

            var jsonObject = CompileCatalogue(cat);

            var catJsonTmp = new FileInfo(Path.Combine(cDir.FullName, $"cat-{config.CatalogueId}-{ObjectId.NewObjectId()}.json"));

            File.WriteAllText(catJsonTmp.FullName, JsonConvert.SerializeObject(jsonObject, Formatting.Indented));

            L.Info($"{catJsonTmp.FullName} catalogue written.");

            var response = M.SendAndWait<GetContentUriRequest,GetContentUriResponse>(new GetContentUriRequest(catJsonTmp.FullName));
            if (response == null)
            {
                L.Fatal("Unable to add the catalogue file to IPFS. Aborting");
                return null;
            }

            var catJsonFinal = new FileInfo(Path.Combine(cDir.FullName, $"cat-{config.CatalogueId}-{response.ContentUri.Path}.json"));

            if (catJsonFinal.Exists)
                catJsonFinal.Delete();

            File.Move(catJsonTmp.FullName, catJsonFinal.FullName);

            L.Log("Catalogue IPFS HASH: " + response.ContentUri.Path);
            L.Log("Catalogue: " + catJsonFinal.FullName);

            return jsonObject;
        }

        private CatalogueBuild Discover(List<DirectoryInfo> dirs)
        {
            var cat = new CatalogueBuild();
            foreach (var d in dirs)
            {
                foreach (var sd in d.GetDirectories())
                {
                    try
                    {
                        var entry = CataloguePackageBuilder.Rebuild(sd);
                        if (entry != null)
                            cat.Add(entry);
                    }
                    catch (Exception e)
                    {
                        L.Error(e.Message + " in " + sd.FullName);
                    }
                }
            }

            return cat;
        }

        private Catalogue CompileCatalogue(CatalogueBuild catalogueBuild)
        {
            var cs = new Catalogue {Owner = "prime"};

            foreach (var group in catalogueBuild.GroupBy(x=>x.MetaData.Id).Where(x=>x.Any()))
            {
                var ces = new CataloguePackage() { Id = group.Key, Title = group.First().MetaData?.Title };
                cs.Entries.Add(ces);

                foreach (var i in group)
                    ces.Instances.Add(CreateInstance(i));
            }

            return cs;
        }

        private CatalogueInstance CreateInstance(CataloguePackageBuilder builder)
        {
            var instance = new CatalogueInstance(builder.MetaData)
            {
                ContentUri = ContentUri.AddFromLocalDirectory(C.M, builder.Source)
            };
            return instance;
        }
    }
}