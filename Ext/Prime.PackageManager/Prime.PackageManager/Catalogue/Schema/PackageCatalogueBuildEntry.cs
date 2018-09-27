using System.IO;
using Prime.Core;

namespace Prime.PackageManager
{
    public class PackageCatalogueBuildEntry
    {
        public static string MetaName = "prime-ext.json";

        public PackageMeta MetaData { get; }

        public DirectoryInfo Source { get; }

        public PackageCatalogueBuildEntry(PackageMeta metaData, DirectoryInfo source) { 
            MetaData = metaData;
            Source = source;
        }
    }
}