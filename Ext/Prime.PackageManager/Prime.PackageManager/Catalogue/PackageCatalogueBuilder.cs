using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Prime.Core;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Radiant;

namespace Prime.PackageManager
{
    public class PackageCatalogueBuilder : CommonBase, ICatalogueBuilder
    {
        private PackageCatalogueBuilder() : base((PrimeContext)null) { }

        public string TypeName => "packages";
        private readonly CatalogueBuilder _builder;

        public PackageCatalogueBuilder(PrimeInstance prime, CataloguePublisherConfig config) : base(prime.C)
        {
            _builder = new CatalogueBuilder(prime, this, config);
        }

        public ContentUri Build()
        {
            return _builder.Build();
        }

        public ICatalogue CompileCatalogue()
        {
            var dDir = C.FileSystem.DistributionDirectory;

            var dirs = dDir.EnumerateDirectories().ToList();

            L.Info("Scanning distribution directory: " + dDir.FullName);

            if (dirs.Count == 0)
            {
                L.Warn("Distribution directory is empty.");
                return null;
            }

            var build = Discover(dirs);

            L.Info($"{build.Count} entries found, compiling package catalogue..");

            return CompileCatalogue(build);
        }

        public ICatalogue CompileCatalogue(PackageCatalogueBuildEntries packageCatalogueBuildEntries)
        {
            var cs = new PackageCatalogue { Owner = "prime" };

            foreach (var group in packageCatalogueBuildEntries.GroupBy(x => x.MetaData.Id).Where(x => x.Any()))
            {
                var ces = new PackageEntry() { Id = group.Key, Title = group.First().MetaData?.Title };
                cs.Packages.Add(ces);

                foreach (var i in group)
                    ces.PackageInstances.Add(CreateInstance(i));
            }

            L.Info("Package catalogue compiled.");

            return cs;
        }

        public static PackageCatalogueBuildEntry ReadPackageMetaInfo(DirectoryInfo source)
        {
            var metaFile = new FileInfo(Path.Combine(source.FullName, PackageCatalogueBuildEntry.MetaName));
            if (!metaFile.Exists)
                return null;

            var meta = JsonConvert.DeserializeObject<PackageMeta>(File.ReadAllText(metaFile.FullName));

            var ext = new PackageMeta() {Title = meta.Title, Id = meta.Id, Platform = meta.Platform, Version = meta.Version};

            return new PackageCatalogueBuildEntry(ext, source);
        }

        private PackageCatalogueBuildEntries Discover(List<DirectoryInfo> dirs)
        {
            var cat = new PackageCatalogueBuildEntries();
            foreach (var d in dirs)
            {
                foreach (var sd in d.GetDirectories())
                {
                    try
                    {
                        var entry = ReadPackageMetaInfo(sd);
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

        public PackageInstance CreateInstance(PackageCatalogueBuildEntry builder)
        {
            var instance = new PackageInstance(builder.MetaData)
            {
                ContentUri = ContentUri.AddFromLocalDirectory(C.M, builder.Source)
            };
            return instance;
        }
    }
}