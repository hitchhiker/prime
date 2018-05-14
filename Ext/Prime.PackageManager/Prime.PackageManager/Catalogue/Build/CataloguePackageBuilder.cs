using System;
using System.IO;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;

namespace Prime.PackageManager
{
    public class CataloguePackageBuilder
    {
        public CataloguePackageBuilder(PackageMeta metaData, DirectoryInfo source)
        {
            MetaData = metaData;
            Source = source;
        }

        public PackageMeta MetaData { get; }

        public DirectoryInfo Source { get; }

        public static string MetaName = "prime-ext.json";

        public static CataloguePackageBuilder Rebuild(DirectoryInfo source)
        {
            var metaFile = new FileInfo(Path.Combine(source.FullName, MetaName));
            if (!metaFile.Exists)
                return null;

            var meta = JsonConvert.DeserializeObject<PackageMeta>(File.ReadAllText(metaFile.FullName));

            var ext = new PackageMeta() {Title = meta.Title, Id = meta.Id, Platform = meta.Platform, Version = meta.Version};

            return new CataloguePackageBuilder(ext, source);
        }
    }
}