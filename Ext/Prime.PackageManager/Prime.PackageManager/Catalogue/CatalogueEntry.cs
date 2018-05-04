using System;
using System.IO;
using LiteDB;
using Newtonsoft.Json;
using Prime.Core;

namespace Prime.PackageManager
{
    public class CatalogueEntry
    {
        public CatalogueEntry(PackageMeta metaData, DirectoryInfo source)
        {
            MetaData = metaData;
            Source = source;
        }

        public PackageMeta MetaData { get; }

        public DirectoryInfo Source { get; }

        public static string MetaName = "prime-ext.json";

        public static CatalogueEntry Rebuild(DirectoryInfo source)
        {
            var metaFile = new FileInfo(Path.Combine(source.FullName, MetaName));
            if (!metaFile.Exists)
                return null;

            var meta = JsonConvert.DeserializeObject<PackageMetaSchema>(File.ReadAllText(metaFile.FullName));

            var ext = new PackageMeta() {Title = meta.Title, Id = new ObjectId(meta.Id), Platform = meta.Platform.ToEnum<Platform>(), Version = Version.Parse(meta.Version)};

            return new CatalogueEntry(ext, source);
        }
    }
}